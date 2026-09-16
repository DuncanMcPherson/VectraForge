using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        if (_recentProjects.Count == 0)
        {
            _recentProjects.Add(project);
            RecentProjectsChanged?.Invoke(this, _recentProjects);
            return;
        }

        var updated = _recentProjects[..];
        var index = updated.IndexOf(project);
        updated.RemoveAt(index);
        updated.Insert(index, project);
        var recentsList = new RecentProjectsList()
        {
            Projects = updated
        };
        configService.WriteConfigFile("recent_projects.json", recentsList.ToJson());
        
        if (_recentProjects[0] == project)
            return;
        var projectIndex = _recentProjects.IndexOf(project);
        if (projectIndex != -1)
            _recentProjects.RemoveAt(projectIndex);
        _recentProjects.Insert(0, project);
        RecentProjectsChanged?.Invoke(this, _recentProjects);
    }
}