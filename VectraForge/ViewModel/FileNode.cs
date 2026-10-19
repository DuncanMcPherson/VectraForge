using System.Collections.ObjectModel;

namespace VectraForge.ViewModel;

public class FileNode : ViewModelBase
{
    private bool _loaded;
    
    public string Path { get; }
    public string Name { get; }
    public bool IsDirectory { get; }
    public ObservableCollection<FileNode> Children { get; } = [];

    public FileNode(string path, bool isDirectory, string? displayName = null)
    {
        Path = path;
        IsDirectory = isDirectory;
        Name = displayName ?? System.IO.Path.GetFileName(path);
        if (isDirectory)
            Children.Add(new FileNode(string.Empty, false));
    }

    public bool IsExpanded
    {
        get;
        set
        {
            if (SetField(ref field, value) && value)
                Load();
        }
    }

    private void Load()
    {
        if (_loaded || !IsDirectory) return;
        _loaded = true;
        Children.Clear();
        try
        {
            var entries = new DirectoryInfo(Path).EnumerateFileSystemInfos()
                .Where(e => !(e is DirectoryInfo && e.Name.StartsWith('.')))
                .OrderByDescending(e => e is DirectoryInfo)
                .ThenBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
            foreach (var e in entries)
                Children.Add(new FileNode(e.FullName, e is DirectoryInfo));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Ignore
        }
    }
}