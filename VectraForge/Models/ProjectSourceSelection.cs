namespace VectraForge.Models;

public record ProjectSourceSelection(ProjectSourceKind Kind, string Path);

public enum ProjectSourceKind
{
    Folder,
    File,
    Cancelled
}