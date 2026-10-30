using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using Dock.Settings;
using VectraForge.Models;
using VectraForge.Services.Interfaces;
using VectraForge.ViewModel;

namespace VectraForge.Services;

public class DockFactory(IToolWindowRegistry registry, Func<ProjectContext?> context) : Factory
{
    private readonly Dictionary<string, ViewModelBase> _toolContexts = new();
    private IDocumentDock? _documentDock;
    private IRootDock? _root;
    public Func<EditorFileViewModel, Task<bool>>? ConfirmClose { get; set; }
    private bool _closeApproved;
    private readonly HashSet<IDockable> _pendingClose = [];
    
    public IReadOnlyList<EditorFileViewModel> OpenEditors() =>
        [.. AllDockables(_root).Select(d => d.Context).OfType<EditorFileViewModel>().Distinct()];

    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows() || OperatingSystem.IsMacOS()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    public override bool OnDockableClosing(IDockable? dockable)
    {
        if (_closeApproved || dockable?.Context is not EditorFileViewModel { IsDirty: true } vm)
            return base.OnDockableClosing(dockable);
        if (_pendingClose.Add(dockable)) _ = CloseAfterConfirmAsync(dockable, vm);
        return false;
    }

    private async Task CloseAfterConfirmAsync(IDockable? dockable, EditorFileViewModel vm)
    {
        try
        {
            if (ConfirmClose is null || !await ConfirmClose(vm)) return;
            _closeApproved = true;
            try
            {
                CloseDockable(dockable);
            }
            finally
            {
                _closeApproved = false;
            }
        }
        finally
        {
            _pendingClose.Remove(dockable!);
        }
    }

    public override IRootDock CreateLayout()
    {
        var tools = registry.All.ToLookup(t => t.DefaultSlot);
        var left = CreateToolDock(tools[WindowSlot.Left], Alignment.Left, 0.2);
        var right = CreateToolDock(tools[WindowSlot.Right], Alignment.Right, 0.2);
        var bottom = CreateToolDock(tools[WindowSlot.Bottom], Alignment.Bottom, 0.25);

        var documents = new DocumentDock
        {
            Title = "Documents",
            IsCollapsable = false,
            VisibleDockables = CreateList<IDockable>(),
            EnableWindowDrag = true
        };

        IDockable center = bottom is null
            ? documents
            : new ProportionalDock
            {
                Orientation = Orientation.Vertical,
                VisibleDockables = CreateList<IDockable>(documents, new ProportionalDockSplitter(), bottom)
            };
        var columns = new List<IDockable>();
        if (left is not null)
        {
            columns.Add(left);
            columns.Add(new ProportionalDockSplitter());
        }
        columns.Add(center);
        if (right is not null)
        {
            columns.Add(new ProportionalDockSplitter());
            columns.Add(right);
        }

        var main = new ProportionalDock
        {
            Orientation = Orientation.Horizontal,
            VisibleDockables = CreateList<IDockable>([.. columns])
        };

        var root = CreateRootDock();
        root.IsCollapsable = false;
        root.VisibleDockables = CreateList<IDockable>(main);
        root.ActiveDockable = main;
        root.DefaultDockable = main;
        root.LeftPinnedDockables = CreateList<IDockable>();
        root.RightPinnedDockables = CreateList<IDockable>();
        root.TopPinnedDockables = CreateList<IDockable>();
        root.BottomPinnedDockables = CreateList<IDockable>();
        root.PinnedDock = null;
        _documentDock = documents;
        _root = root;
        return root;
    }

    public bool TryActivateDocument(string fullPath)
    {
        var doc = AllDockables(_root).FirstOrDefault(d =>
            d.Context is EditorFileViewModel e && PathComparer.Equals(e.FilePath, fullPath));
        if (doc is null) return false;
        Activate(doc);
        return true;
    }

    private static IEnumerable<IDockable> AllDockables(IDockable? node)
    {
        if (node is null) yield break;
        yield return node;
        if (node is IDock { VisibleDockables: {} children})
            foreach (var child in children)
                foreach (var d in AllDockables(child))
                    yield return d;
        if (node is IRootDock { Windows: {} windows })
            foreach (var window in windows)
            foreach (var d in AllDockables(window.Layout))
                yield return d;
    }

    public void AddDocument(EditorFileViewModel vm)
    {
        if (_documentDock is null) return;
        var doc = new Document { Id = $"doc:{vm.FilePath}", Title = vm.DisplayTitle, Context = vm, CanClose = true };
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(EditorFileViewModel.DisplayTitle)) doc.Title = vm.DisplayTitle;
        };
        AddDockable(_documentDock, doc);
        Activate(doc);
    }

    private void Activate(IDockable dock)
    {
        SetActiveDockable(dock);
        SetFocusedDockable((dock.Owner as IDock) ?? _documentDock!, dock);
    }

    public override IDockWindow? CreateWindowFrom(IDockable dockable)
    {
        var window = base.CreateWindowFrom(dockable);
        window?.Title = "VectraForge";
        return window;
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = registry.All.ToDictionary(t => t.Id, t => (Func<object?>)(() => GetOrCreateContext(t)));
        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => DockSettings.UseManagedWindows ? new ManagedHostWindow() : new HostWindow()
        };
        
        base.InitLayout(layout);
    }

    private IToolDock? CreateToolDock(IEnumerable<ToolWindow> slotTools, Alignment alignment, double proportion)
    {
        var docked = slotTools.Select(t => (IDockable)new Tool { Id = t.Id, Title = t.Title, CanClose = false })
            .ToArray();
        return docked.Length == 0
            ? null
            : new ToolDock
            {
                ActiveDockable = docked[0],
                VisibleDockables = CreateList(docked),
                Alignment = alignment,
                Proportion = proportion
            };
    }

    private ViewModelBase GetOrCreateContext(ToolWindow tool)
    {
        if (!_toolContexts.TryGetValue(tool.Id, out var vm))
            _toolContexts[tool.Id] = vm = tool.CreateView(context());
        return vm;
    }
}