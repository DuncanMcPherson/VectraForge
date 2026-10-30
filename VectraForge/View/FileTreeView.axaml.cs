using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using VectraForge.ViewModel;

namespace VectraForge.View;

public partial class FileTreeView : UserControl
{
    public FileTreeView()
    {
        InitializeComponent();
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is FileTreeViewModel vm && e.Source is StyledElement { DataContext: FileNode node})
            vm.Open(node);
    }
}