using System.Collections.ObjectModel;
using VectraForge.Services;

namespace VectraForge.ViewModel;

public class FileTreeViewModel : ViewModelBase
{
    public ObservableCollection<FileNode> Roots { get; } = [];
    public bool IsEmpty => Roots.Count == 0;

    public FileTreeViewModel(ProjectContext? context)
    {
        if (context is null) return;
        
        var root = context.Kind == ProjectKind.File
            ? Path.GetDirectoryName(context.RootPath)!
            : context.RootPath;
        Roots.Add(new FileNode(root, true, context.Name));
    }
}