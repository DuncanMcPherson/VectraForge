using VectraForge.Models;
using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

/// <inheritdoc/>
public class RecentProjectService(IConfigService configService) : IRecentProjectService
{
    /// <inheritdoc/>
    public event EventHandler<IReadOnlyList<RecentProject>>? RecentProjectsChanged;
    private List<RecentProject> _recentProjects = [];
    
    /// <inheritdoc/>
    public async Task<IReadOnlyList<RecentProject>> GetRecentProjects()
    {
        var storedConfig = await configService.ReadConfigFile("recent_projects.json");
        if (string.IsNullOrEmpty(storedConfig))
            return [];
        
        var recentProjectsList = RecentProjectsList.FromJson(storedConfig);
        _recentProjects = recentProjectsList?.Projects ?? [];
        return _recentProjects;
    }

    /// <inheritdoc/>
    public void NotifyProjectOpened(RecentProject project)
    {
        project = project with { LastOpened = DateTimeOffset.Now };
        var updated = _recentProjects[..];
        var index = updated.IndexOf(project);
        if (index != -1)
            updated.RemoveAt(index);
        updated.Insert(0, project);

        Commit(updated);
    }

    /// <inheritdoc/>
    public void RemoveProject(RecentProject project)
    {
        var updated = _recentProjects[..];
        if (updated.RemoveAll(p => p.Equals(project)) == 0)
            return;
        Commit(updated);
    }

    private void Commit(List<RecentProject> updated)
    {
        var recentsList = new RecentProjectsList { Projects = updated };
        configService.WriteConfigFile("recent_projects.json", recentsList.ToJson());
        _recentProjects = updated;
        RecentProjectsChanged?.Invoke(this, _recentProjects);
    }
}