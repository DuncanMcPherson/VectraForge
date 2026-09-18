using System.Collections.Generic;
using System.Text.Json;

namespace VectraForge.Models;

/// <summary>
/// Represents a collection of recently accessed projects along with their metadata.
/// </summary>
public class RecentProjectsList
{
    /// <summary>
    /// Gets the schema version used to define the structure of the
    /// recent projects list. This value ensures compatibility between
    /// different versions of the application handling serialized data.
    /// </summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>
    /// Gets or sets the list of recently accessed projects.
    /// Each project in the list is represented by an instance of <see cref="RecentProject"/>,
    /// containing metadata such as the project's path, name, and last-accessed time.
    /// </summary>
    public List<RecentProject> Projects { get; set; } = [];

    /// <summary>
    /// Deserializes a JSON string into an instance of the <see cref="RecentProjectsList"/> class.
    /// </summary>
    /// <param name="json">The JSON string representing a list of recent projects.</param>
    /// <returns>
    /// An instance of <see cref="RecentProjectsList"/> if the deserialization is successful; otherwise, null.
    /// </returns>
    public static RecentProjectsList? FromJson(string json)
    {
        return JsonSerializer.Deserialize<RecentProjectsList>(json);
    }
    
    public string ToJson() => JsonSerializer.Serialize(this);
}