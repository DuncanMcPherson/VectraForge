using System.Collections.ObjectModel;
using VectraForge.Services;
using VectraForge.Services.Interfaces;

namespace VectraForge.ViewModel;

public class FileTreeViewModel : ViewModelBase
{
    public ObservableCollection<FileNode> Roots { get; } = [];
    public bool IsEmpty => Roots.Count == 0;

    private readonly IEditorService _editor;

    public FileTreeViewModel(ProjectContext? context, IEditorService editor)
    {
        _editor = editor;
        
        if (context is null) return;
        
        var root = context.Kind == ProjectKind.File
            ? Path.GetDirectoryName(context.RootPath)!
            : context.RootPath;
        Roots.Add(new FileNode(root, true, context.Name));
    }

    public void Open(FileNode node)
    {
        if (node.IsDirectory || string.IsNullOrWhiteSpace(node.Path)) return;
        _editor.RequestOpen(node.Path);
    }
}