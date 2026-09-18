using VectraForge.Models;

namespace VectraForge.Services;

public sealed record ProjectContext(string RootPath, ProjectKind Kind);

public enum ProjectKind
{
    File,
    Folder
}