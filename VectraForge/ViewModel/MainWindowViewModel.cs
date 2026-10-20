using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using VectraForge.Commands;
using VectraForge.Models;
using VectraForge.Services;
using VectraForge.Services.Interfaces;
using VectraForge.Windows.Dialogs;

namespace VectraForge.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectContextAccessor _contextAccessor;
    private readonly IProjectLayoutStore _layoutStore;
    private readonly IToolWindowRegistry _registry;
    private readonly IWindowManager _windowManager;
    private ProjectContext? _context;
    private (ProjectContext Context, LayoutState State)? _pending;
    private readonly DispatcherTimer _saveTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };
    private bool _isApplying;
    
    public IReadOnlyList<SlotViewModel> Slots { get; }
    public SlotViewModel Left => SlotFor(WindowSlot.Left);
    public SlotViewModel Right => SlotFor(WindowSlot.Right);
    public SlotViewModel Bottom => SlotFor(WindowSlot.Bottom);

    public string WindowTitle => _context is not null ? $"VectraForge — {_context.Name}" : "VectraForge";
    
    public ICommand ShowAboutCommand { get; }

    public MainWindowViewModel(IProjectContextAccessor contextAccessor, IProjectLayoutStore layoutStore, IToolWindowRegistry registry, IWindowManager windowManager)
    {
        _contextAccessor = contextAccessor;
        _layoutStore = layoutStore;
        _registry = registry;
        _windowManager = windowManager;
        _context = _contextAccessor.Current;
        ShowAboutCommand = new AsyncRelayCommand(() => _windowManager.ShowDialogWindow<AboutDialog, object>(this));

        _saveTimer.Tick += (_, _) => FlushLayout();
        Slots = [.. Enum.GetValues<WindowSlot>().Select(s => new SlotViewModel(s, () => _context, OnLayoutChanged))];
        ApplyLayout(_context is null ? null : _layoutStore.Load(_context));
    }

    private void OnLayoutChanged()
    {
        if (_isApplying || _context is null) return;
        _pending = (_context, SnapshotLayout());
        _saveTimer.Stop();
        _saveTimer.Start();
    }

    public void OnLoaded(Window window)
    {
        _windowManager.RegisterDialogOwner(this, window);
    }

    public void OnClose()
    {
        _windowManager.UnregisterDialogOwner(this);
    }

    public void FlushLayout()
    {
        _saveTimer.Stop();
        if (_pending is not { } p) return;
        _pending = null;
        _layoutStore.Save(p.Context, p.State);
    }

    public void SwitchProject(ProjectContext next)
    {
        FlushLayout();
        _context = next;
        _contextAccessor.Current = next;
        OnPropertyChanged(nameof(WindowTitle));
        ApplyLayout(_layoutStore.Load(next));
    }

    private void ApplyLayout(LayoutState? saved)
    {
        var state = LayoutState.Reconcile(saved, _registry.All);
        _isApplying = true;
        try
        {
            foreach (var slot in Slots)
                slot.Load(state.Slots[slot.Slot], _registry);
        }
        finally
        {
            _isApplying = false;
        }
    }

    private LayoutState SnapshotLayout() => new()
    {
        Slots = Slots.ToDictionary(s => s.Slot, s => s.ToState())
    };

    private SlotViewModel SlotFor(WindowSlot slot) => Slots.First(s => s.Slot == slot);
}