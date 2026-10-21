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
        return root;
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