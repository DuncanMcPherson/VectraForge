using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using VectraForge.ViewModel;

namespace VectraForge.Windows;

[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel vm)
        : this()
    {
        DataContext = vm;
        Closing += (_, _) => vm.FlushLayout();
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public MainWindow()
    {
        InitializeComponent();
    }
}