using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VectraForge.Models;

namespace VectraForge.Services.Interfaces;

/// <summary>
/// Provides functionality for managing and retrieving information about recently accessed projects.
/// </summary>
public interface IRecentProjectService
{
    /// <summary>
    /// Event that is triggered whenever the collection of recent projects changes.
    /// Subscribers to this event can handle updates to the list of recent projects,
    /// such as displaying the updated list in the user interface or performing other
    /// necessary actions in response to this change.
    /// </summary>
    event EventHandler<IReadOnlyList<RecentProject>> RecentProjectsChanged;

    /// <summary>
    /// Retrieves a list of recently accessed projects.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a read-only list of <see cref="RecentProject"/> objects, representing each
    /// recently accessed project with relevant metadata such as path, name, and last access time.
    /// </returns>
    Task<IReadOnlyList<RecentProject>> GetRecentProjects();

    /// <summary>
    /// Notifies the service that a project has been opened, updating the internal list of recent projects
    /// and triggering the <see cref="RecentProjectsChanged"/> event if necessary.
    /// </summary>
    /// <param name="project">
    /// The <see cref="RecentProject"/> instance representing the project that was opened.
    /// This includes metadata such as its path, name, and the time it was last accessed.
    /// </param>
    void NotifyProjectOpened(RecentProject project);
}