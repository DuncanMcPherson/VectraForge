using System.Collections.Concurrent;
using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

// TODO: This class is not yet tested because it is mid-refactor on a different branch. write tests when the refactor is complete
/// <inheritdoc/>
public class ConfigService : IConfigService
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    /// <inheritdoc/>
    public string ConfigFolder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VectraForge");

    /// <inheritdoc/>
    public async Task WriteConfigFile(string fileName, string content)
    {
        Directory.CreateDirectory(ConfigFolder);
        var gate = _locks.GetOrAdd(fileName, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();

        try
        {
            var target = Path.Combine(ConfigFolder, fileName);
            var tmp = Path.Join(ConfigFolder, $"{fileName}.{Guid.NewGuid():N}.tmp");
            await File.WriteAllTextAsync(tmp, content);
            File.Move(tmp, target, true);
        }
        finally
        {
            gate.Release();
        }
    }

    /// <inheritdoc/>
    public async Task<string> ReadConfigFile(string fileName)
    {
        Directory.CreateDirectory(ConfigFolder);
        var path = Path.Combine(ConfigFolder, fileName);
        if (!File.Exists(path))
        {
            return string.Empty;
        }
        var gate = _locks.GetOrAdd(fileName, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();

        try
        {
            return await File.ReadAllTextAsync(path);
        }
        finally
        {
            gate.Release();
        }
    }
}