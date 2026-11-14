using System.Collections.Concurrent;
using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

public class AtomicFileStore : IAtomicFileStore
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new(StringComparer.OrdinalIgnoreCase);
    private SemaphoreSlim Gate(string path) => _gates.GetOrAdd(path, _ => new SemaphoreSlim(1, 1));

    public void Write(string path, string content)
    {
        var full = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        var tmp = $"{full}.{Guid.NewGuid():N}.tmp";
        var gate = Gate(full);
        gate.Wait();
        try
        {
            File.WriteAllText(tmp, content);
            File.Move(tmp, full, true);
        }
        catch
        {
            try
            {
                File.Delete(tmp);
            }
            catch
            {
                // ignored
            }

            throw;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task WriteAsync(string path, string content)
    {
        var full = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        var tmp = $"{full}.{Guid.NewGuid():N}.tmp";
        var gate = Gate(full);
        await gate.WaitAsync();
        try
        {
            await File.WriteAllTextAsync(tmp, content);
            File.Move(tmp, full, true);
        }
        catch
        {
            try
            {
                File.Delete(tmp);
            }
            catch
            {
                // Ignored
            }

            throw;
        }
        finally
        {
            gate.Release();
        }
    }

    public string? Read(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
        {
            return null;
        }

        var gate = Gate(full);
        gate.Wait();
        try
        {
            return File.ReadAllText(full);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<string?> ReadAsync(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
        {
            return null;
        }

        var gate = Gate(full);
        await gate.WaitAsync();
        try
        {
            return await File.ReadAllTextAsync(full);
        }
        finally
        {
            gate.Release();
        }
    }
}