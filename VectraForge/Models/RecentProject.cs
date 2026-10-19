using System.Diagnostics.CodeAnalysis;
using VectraForge.Services;

namespace VectraForge.Models;

/// <summary>
/// Represents a recently accessed project, including associated metadata
/// such as its path, name, and the last time it was opened.
/// </summary>
public sealed record RecentProject
{
    /// <summary>
    /// Gets or sets the file system path of the recent project.
    /// </summary>
    /// <remarks>
    /// This property represents the location of the project in the file system.
    /// It is a required property and must be initialized with a valid string value.
    /// </remarks>
    public required string Path { get; init; }

    /// <summary>
    /// Gets or sets the name of the recent project.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the date and time when the project was last opened.
    /// </summary>
    /// <remarks>
    /// This property indicates the most recent point in time the project was accessed.
    /// It is particularly useful for tracking user activity or ordering recent projects.
    /// </remarks>
    public DateTimeOffset LastOpened { get; init; }
    
    public ProjectKind Kind { get; init; }
    

    public static RecentProject FromContext(ProjectContext context)
    {
        var path = context.RootPath;
        var name = context.Kind switch
        {
            ProjectKind.Folder => new DirectoryInfo(path).Name,
            ProjectKind.File => System.IO.Path.GetFileNameWithoutExtension(path),
            _ => throw new NotSupportedException($"Unknown project source kind: {context.Kind}")
        };
        return new RecentProject
        {
            LastOpened = DateTimeOffset.Now,
            Name = name,
            Path = path,
            Kind = context.Kind
        };
    }

    public ProjectContext ToContext()
    {
        return new ProjectContext(Path, Name, Kind);
    }

    /// <summary>
    /// Determines whether the specified <see cref="RecentProject"/> object is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="RecentProject"/> instance to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="RecentProject"/> is equal to the current instance; otherwise, <c>false</c>.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public bool Equals(RecentProject? other) =>
        other is not null && string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Serves as the default hash function for the <see cref="RecentProject"/> record.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer hash code derived from the <see cref="Path"/> property,
    /// using a case-insensitive string comparison.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Path);
}