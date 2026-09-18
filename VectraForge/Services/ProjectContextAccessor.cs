using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

public sealed class ProjectContextAccessor : IProjectContextAccessor
{
    public ProjectContext? Current { get; set; }
}