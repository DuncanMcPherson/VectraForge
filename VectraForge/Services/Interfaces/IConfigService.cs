using System.Threading.Tasks;

namespace VectraForge.Services.Interfaces;

/// <summary>
/// Provides methods for managing configuration files and retrieving configuration folder paths.
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Gets the absolute file path of the configuration folder used by the application.
    /// This property typically references a directory within the user's application data folder
    /// where configuration files are stored and managed.
    /// </summary>
    string ConfigFolder { get; }

    /// <summary>
    /// Writes a configuration file to the specified configuration folder. This process ensures atomicity
    /// by creating a temporary file before renaming it to the target file name.
    /// </summary>
    /// <param name="fileName">The name of the configuration file to write, without the temporary extension.</param>
    /// <param name="content">The content to be written to the configuration file.</param>
    Task WriteConfigFile(string fileName, string content);

    /// <summary>
    /// Reads the contents of a specified configuration file from the configured directory.
    /// </summary>
    /// <param name="fileName">The name of the configuration file to be read.</param>
    /// <returns>The content of the specified configuration file as a string.</returns>
    Task<string?> ReadConfigFile(string fileName);
}