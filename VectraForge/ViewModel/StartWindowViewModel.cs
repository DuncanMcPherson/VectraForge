using System.Windows.Input;
using Avalonia.Controls;
using VectraForge.Commands;
using VectraForge.Models;
using VectraForge.Services;
using VectraForge.Services.Interfaces;
using VectraForge.Windows.Dialogs;

namespace VectraForge.ViewModel;

/// <summary>
/// Represents the view model for the start window of the application.
/// Provides data and functionality to manage and display recent projects in the UI.
/// </summary>
public class StartWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Represents a collection of recently accessed projects displayed in the start window.
    /// This property retrieves a read-only list of <see cref="RecentProject"/> instances,
    /// providing metadata such as the project's path, name, and the last-accessed date.
    /// </summary>
    /// <remarks>
    /// The property is populated during the window's loading process by invoking the
    /// <see cref="IRecentProjectService.GetRecentProjects"/> method. It updates dynamically
    /// in response to changes raised by the <see cref="IRecentProjectService.RecentProjectsChanged"/> event.
    /// </remarks>
    public IReadOnlyList<RecentProject> RecentProjects
    {
        get;
        private set => SetField(ref field, value);
    } = [];

    /// <summary>
    /// Gets the command to open a recently accessed project from the list of recent projects.
    /// When executed, the command triggers the logic to load the selected project
    /// and transition the application to its corresponding workspace or environment.
    /// </summary>
    /// <remarks>
    /// This command is typically bound to UI elements such as buttons or menu items
    /// in the start window, facilitating user interaction with the list of recent projects.
    /// It expects a <see cref="RecentProject"/> instance as a parameter, which provides
    /// the necessary details about the project to be opened.
    /// 
    /// The command is initialized during the construction of the <see cref="StartWindowViewModel"/>
    /// and uses the <see cref="RelayCommand{T}"/> implementation to encapsulate the execution logic.
    /// </remarks>
    public ICommand OpenRecentProjectCommand { get; private set; } = null!;

    public ICommand OpenProjectCommand { get; private set; } = null!;
    public ICommand CloneProjectCommand { get; private set; } = null!;
    public ICommand CreateProjectCommand { get; private set; } = null!;
    public ICommand ContinueWithoutCodeCommand { get; private set; } = null!;

    /// <summary>
    /// Represents the service responsible for managing and providing access to recently opened projects.
    /// Provides functionality such as retrieving recent projects and reacting to changes in the list of recent projects.
    /// </summary>
    private readonly IRecentProjectService _recentProjectService;

    private readonly IWindowManager _windowManager;

    /// <summary>
    /// Represents the ViewModel for the start window of the application.
    /// It manages the state of recent projects and provides interaction logic for the start window.
    /// </summary>
    public StartWindowViewModel(IRecentProjectService recentProjectService, IWindowManager windowManager)
    {
        _recentProjectService = recentProjectService;
        _windowManager = windowManager;
        InitializeCommands();
        _recentProjectService.RecentProjectsChanged += OnRecentProjectsChanged;
    }

    private void InitializeCommands()
    {
        OpenRecentProjectCommand = new RelayCommand<RecentProject>(OpenRecentProject);
        OpenProjectCommand = new AsyncRelayCommand(OpenProject);
        CloneProjectCommand = new RelayCommand(() => { }, () => false);
        CreateProjectCommand = new RelayCommand(() => { }, () => false);
        ContinueWithoutCodeCommand = new RelayCommand(() => { }, () => false);
    }

    /// <summary>
    /// Handles cleanup logic when the associated `StartWindow` is closing.
    /// This method is responsible for detaching event handlers and releasing resources
    /// allocated during the lifecycle of the `StartWindowViewModel`.
    /// </summary>
    public void OnClosing()
    {
        _recentProjectService.RecentProjectsChanged -= OnRecentProjectsChanged;
        _windowManager.UnregisterDialogOwner(this);
    }

    private void OpenRecentProject(RecentProject? recentProject)
    {
        if (recentProject is null) return;
        _recentProjectService.NotifyProjectOpened(recentProject);
        var context = recentProject.ToContext();
        _windowManager.ShowProjectWindow(context);
    }

    private async Task OpenProject()
    {
        var result = await _windowManager.ShowDialogWindow<ProjectPickerDialog, ProjectSourceSelection>(this);
        if (result is null || result.Kind == ProjectSourceKind.Cancelled || string.IsNullOrEmpty(result.Path))
            return;

        switch (result.Kind)
        {
            case ProjectSourceKind.Folder:
            {
                var context = new ProjectContext(result.Path, result.Kind.ToProjectKind());
                _windowManager.ShowProjectWindow(context);
                RegisterRecentProject(RecentProject.FromContext(context));
                break;
            }
            case ProjectSourceKind.File:
                // TODO: open a file, and process it with the project system (non-existent as of yet)
                throw new NotSupportedException("Opening packages and modules is not supported yet");
            case ProjectSourceKind.Cancelled:
            default:
                throw new NotSupportedException($"Unhandled project source kind: {result.Kind}");
        }
    }

    private void RegisterRecentProject(RecentProject project)
    {
        _recentProjectService.NotifyProjectOpened(project);
    }

    /// <summary>
    /// Asynchronously loads the list of recently accessed projects and updates the <see cref="RecentProjects"/> property.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation of retrieving recently accessed projects
    /// from the associated <see cref="IRecentProjectService"/>.
    /// </returns>
    public async Task OnLoaded(Window window)
    {
        RecentProjects = await _recentProjectService.GetRecentProjects();
        _windowManager.RegisterDialogOwner(this, window);
    }

    /// <summary>
    /// Handles the event triggered when the list of recent projects changes.
    /// Updates the <see cref="RecentProjects"/> property with the new list of recent projects.
    /// </summary>
    /// <param name="sender">The source of the event, which can be null.</param>
    /// <param name="projects">The updated list of recent projects received from the event.</param>
    private void OnRecentProjectsChanged(object? sender, IReadOnlyList<RecentProject> projects)
    {
        RecentProjects = projects;
    }
}