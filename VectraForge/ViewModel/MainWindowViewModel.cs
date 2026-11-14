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
    private readonly IEditorService _editorService;
    private readonly IAtomicFileStore _atomicFileStore;
    private DockFactory? _factory;
    private ProjectContext? _context;

    public IRootDock? Layout
    {
        get;
        set => SetField(ref field, value);
    }

    public string WindowTitle => _context is not null ? $"VectraForge — {_context.Name}" : "VectraForge";

    public ICommand ShowAboutCommand { get; }

    public MainWindowViewModel(IWindowManager windowManager, IProjectContextAccessor contextAccessor,
        IToolWindowRegistry registry,
        IEditorService editorService, IAtomicFileStore atomicFileStore)
    {
        _contextAccessor = contextAccessor;
        _registry = registry;
        _windowManager = windowManager;
        _context = _contextAccessor.Current;
        _editorService = editorService;
        _atomicFileStore = atomicFileStore;
        ShowAboutCommand = new AsyncRelayCommand(() => _windowManager.ShowDialogWindow<AboutDialog, object>(this));
        Layout = BuildLayout();
        _editorService.OpenRequested += OnOpenRequested;
    }

    public void OnLoaded(Window window)
    {
        _windowManager.RegisterDialogOwner(this, window);
    }

    public void OnClose()
    {
        _windowManager.UnregisterDialogOwner(this);
        _editorService.OpenRequested -= OnOpenRequested;
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
        _factory = new DockFactory(_registry, () => _context)
        {
            ConfirmClose = vm => ConfirmDiscardAsync([vm])
        };
        var layout = _factory.CreateLayout();
        _factory.InitLayout(layout);
        return layout;
    }

    public IReadOnlyList<EditorFileViewModel> DirtyEditors() =>
        _factory?.OpenEditors().Where(vm => vm.IsDirty).ToList() ?? [];

    public async Task<bool> ConfirmDiscardAsync(IReadOnlyList<EditorFileViewModel> dirty)
    {
        if (dirty.Count == 0) return true;
        var dialogVm = new UnsavedChangesDialogViewModel([.. dirty.Select(d => Path.GetFileName(d.FilePath))]);
        var choice = await _windowManager.ShowDialogWindow<UnsavedChangesDialog, SaveChoice>(this, dialogVm);
        switch (choice)
        {
            case SaveChoice.Save:
                foreach (var d in dirty)
                    if (!await d.SaveAsync())
                        return false;
                return true;
            case SaveChoice.DontSave:
                return true;
            case SaveChoice.Cancel:
            default:
                return false;
        }
    }

    private void OnOpenRequested(object? sender, string path)
    {
        _ = OpenFileAsync(path);
    }

    private async Task OpenFileAsync(string path)
    {
        var full = Path.GetFullPath(path);
        if (_factory is null || _factory.TryActivateDocument(full)) return;
        var vm = await EditorFileViewModel.LoadAsync(full, _atomicFileStore);
        if (_factory.TryActivateDocument(full)) return;
        _factory.AddDocument(vm);
    }
}