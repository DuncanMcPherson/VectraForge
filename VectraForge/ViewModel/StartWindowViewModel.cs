using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using VectraForge.Commands;
using VectraForge.Models;
using VectraForge.Services.Interfaces;

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
    /// providing metadata such as the project's path, name, and the last accessed date.
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
    
    public ICommand OpenRecentProjectCommand { get; }

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
        OpenRecentProjectCommand = new RelayCommand<RecentProject>(OpenRecentProject);
        _recentProjectService.RecentProjectsChanged += OnRecentProjectsChanged;
    }

    /// <summary>
    /// Handles cleanup logic when the associated `StartWindow` is closing.
    /// This method is responsible for detaching event handlers and releasing resources
    /// allocated during the lifecycle of the `StartWindowViewModel`.
    /// </summary>
    public void OnClosing()
    {
        _recentProjectService.RecentProjectsChanged -= OnRecentProjectsChanged;
    }

    private void OpenRecentProject(RecentProject? recentProject)
    {
        if (recentProject is null) return;
        _recentProjectService.NotifyProjectOpened(recentProject);
        _windowManager.ShowProjectWindow();
    }

    /// <summary>
    /// Asynchronously loads the list of recently accessed projects and updates the <see cref="RecentProjects"/> property.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation of retrieving recently accessed projects
    /// from the associated <see cref="IRecentProjectService"/>.
    /// </returns>
    public async Task OnLoaded()
    {
        RecentProjects = await _recentProjectService.GetRecentProjects();
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