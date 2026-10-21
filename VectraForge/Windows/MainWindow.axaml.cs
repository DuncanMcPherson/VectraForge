using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Interactivity;
using VectraForge.ViewModel;

namespace VectraForge.Windows;

[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
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