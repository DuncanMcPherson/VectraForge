using System.Diagnostics.CodeAnalysis;
using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

[ExcludeFromCodeCoverage]
public sealed class ProjectContextAccessor : IProjectContextAccessor
{
    public ProjectContext? Current { get; set; }
}