using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

/// <inheritdoc/>
public class ConfigService : IConfigService
{
    private readonly IAtomicFileStore _fileStore;
    
    public ConfigService(IAtomicFileStore fileStore)
    {
        _fileStore = fileStore;
    }
    
    /// <inheritdoc/>
    public string ConfigFolder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VectraForge");

    /// <inheritdoc/>
    public async Task WriteConfigFile(string fileName, string content)
    {
        var full = Path.Combine(ConfigFolder, fileName);
        await _fileStore.WriteAsync(full, content);
    }

    /// <inheritdoc/>
    public async Task<string?> ReadConfigFile(string fileName)
    {
        var full = Path.Combine(ConfigFolder, fileName);
        return await _fileStore.ReadAsync(full);
    }
}