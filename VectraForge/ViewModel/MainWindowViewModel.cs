using System.Windows.Input;
using Avalonia.Controls;
using Dock.Model.Controls;
using VectraForge.Commands;
using VectraForge.Services;
using VectraForge.Services.Interfaces;
using VectraForge.Windows.Dialogs;

namespace VectraForge.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IWindowManager _windowManager;
    private readonly IProjectContextAccessor _contextAccessor;
    private readonly IToolWindowRegistry _registry;
    private ProjectContext? _context;

    public IRootDock? Layout
    {
        get;
        set => SetField(ref field, value);
    }
    
    public string WindowTitle => _context is not null ? $"VectraForge — {_context.Name}" : "VectraForge";
    
    public ICommand ShowAboutCommand { get; }

    public MainWindowViewModel(IWindowManager windowManager, IProjectContextAccessor contextAccessor, IToolWindowRegistry registry)
    {
        _contextAccessor = contextAccessor;
        _registry = registry;
        _windowManager = windowManager;
        _context = _contextAccessor.Current;
        ShowAboutCommand = new AsyncRelayCommand(() => _windowManager.ShowDialogWindow<AboutDialog, object>(this));
        Layout = BuildLayout();
    }

    public void OnLoaded(Window window)
    {
        _windowManager.RegisterDialogOwner(this, window);
    }

    public void OnClose()
    {
        _windowManager.UnregisterDialogOwner(this);
    }

    public void SwitchProject(ProjectContext next)
    {
        _context = next;
        _contextAccessor.Current = next;
        OnPropertyChanged(nameof(WindowTitle));
        Layout = BuildLayout();
    }

    private IRootDock BuildLayout()
    {
        var factory = new DockFactory(_registry, () => _context);
        var layout = factory.CreateLayout();
        factory.InitLayout(layout);
        return layout;
    }
}