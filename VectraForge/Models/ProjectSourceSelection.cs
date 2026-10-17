using System.Diagnostics.CodeAnalysis;

namespace VectraForge.Models;

[ExcludeFromCodeCoverage]
public record ProjectSourceSelection(ProjectSourceKind Kind, string Path);

public enum ProjectSourceKind
{
    Folder,
    File,
    Cancelled
}