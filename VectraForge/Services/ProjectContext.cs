namespace VectraForge.Services;

public sealed record ProjectContext(string RootPath, string Name, ProjectKind Kind);

public enum ProjectKind
{
    File,
    Folder
}