using System.Collections.ObjectModel;
using VectraForge.Models;
using VectraForge.Services;
using VectraForge.Services.Interfaces;

namespace VectraForge.ViewModel;

public class SlotViewModel : ViewModelBase
{
    public WindowSlot Slot { get; }
    private readonly Action _onChanged;
    private readonly Dictionary<string, ViewModelBase> _activeToolCache = new();
    private readonly Func<ProjectContext?> _context;
    public ObservableCollection<ToolWindow> Tools { get; } = [];

    public SlotViewModel(WindowSlot slot, Func<ProjectContext?> context, Action onChanged)
    {
        Slot = slot;
        _onChanged = onChanged;
        _context = context;
        Tools.CollectionChanged += (_, _) => Changed();
    }

    public double Size
    {
        get;
        set
        {
            if (SetField(ref field, value))
                Changed();
        }
    }

    public bool IsCollapsed
    {
        get;
        set
        {
            if (SetField(ref field, value))
                Changed();
        }
    }

    public string? ActiveToolId
    {
        get;
        private set
        {
            if (SetField(ref field, value))
            {
                SetTool(value);
                Changed();
            }
        }
    }

    public ViewModelBase? ActiveTool
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool IsExpanded => Tools.Count > 0 && !IsCollapsed;
    
    public double MinExtent => IsExpanded ? LayoutState.MinSize(Slot) : 0;
    public double MaxExtent => IsExpanded ? LayoutState.MaxSize(Slot) : 0;

    public double Extent
    {
        get => IsExpanded ? Size : 0;
        set
        {
            if (!IsExpanded || !double.IsFinite(value) || value < LayoutState.MinSize(Slot)) return;
            Size = Math.Min(value, LayoutState.MaxSize(Slot));
        }
    }

    public void Load(SlotState state, IToolWindowRegistry registry)
    {
        _activeToolCache.Clear();
        ActiveTool = null;
        Size = state.Size;
        IsCollapsed = state.IsCollapsed;
        Tools.Clear();
        foreach (var toolId in state.ToolIds)
        {
            if (registry.TryGet(toolId, out var tool))
                Tools.Add(tool);
        }
        ActiveToolId = state.ActiveToolId;
        SetTool(ActiveToolId);
    }

    public SlotState ToState() => new()
    {
        Size = Size, IsCollapsed = IsCollapsed, ActiveToolId = ActiveToolId,
        ToolIds = [.. Tools.Select(t => t.Id)]
    };

    private void Changed()
    {
        OnPropertyChanged(nameof(Extent));
        OnPropertyChanged(nameof(MinExtent));
        OnPropertyChanged(nameof(MaxExtent));
        OnPropertyChanged(nameof(IsExpanded));
        _onChanged();
    }

    private void SetTool(string? activeTool)
    {
        if (string.IsNullOrEmpty(activeTool))
        {
            ActiveTool = null;
            return;
        }
        if (_activeToolCache.TryGetValue(activeTool, out var tool))
        {
            ActiveTool = tool;
            return;
        }

        var toolWindow = Tools.FirstOrDefault(x => x.Id == activeTool);
        if (toolWindow is null)
            return;
        var vm = toolWindow.CreateView.Invoke(_context());
        _activeToolCache[activeTool] = vm;
        ActiveTool = vm;
    }
}