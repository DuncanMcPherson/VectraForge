namespace VectraForge.Services.Interfaces;

public interface IAtomicFileStore
{
    string? Read(string path);
    Task<string?> ReadAsync(string path);
    void Write(string path, string content);
    Task WriteAsync(string path, string content);
}