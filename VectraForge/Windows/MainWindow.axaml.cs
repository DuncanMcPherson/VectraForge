using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Interactivity;
using VectraForge.ViewModel;

namespace VectraForge.Windows;

[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
    private bool _closeConfirmed;
    public MainWindow(MainWindowViewModel vm)
        : this()
    {
        DataContext = vm;
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        if (_closeConfirmed || DataContext is not MainWindowViewModel vm) return;
        var dirty = vm.DirtyEditors();
        if (dirty.Count == 0) return;
        e.Cancel = true;
        _ = ConfirmAndCloseAsync(vm, dirty);
    }

    private async Task ConfirmAndCloseAsync(MainWindowViewModel vm, IReadOnlyList<EditorFileViewModel> dirty)
    {
        if (!await vm.ConfirmDiscardAsync(dirty)) return;
        _closeConfirmed = true;
        Close();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        (DataContext as MainWindowViewModel)?.OnLoaded(this);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        (DataContext as MainWindowViewModel)?.OnClose();
    }
}