using Avalonia.Controls;
using Avalonia.Interactivity;
using VectraForge.ViewModel;

namespace VectraForge.Windows;

/// <summary>
/// Represents the main start window of the application.
/// This window serves as the entry point for users, allowing interaction with recent projects
/// and navigation to other parts of the application.
/// </summary>
public partial class StartWindow : Window
{
    /// <summary>
    /// Represents the Start Window of the application.
    /// Manages the main UI initialization and interaction logic for bootstrapping the user experience.
    /// </summary>
    public StartWindow(StartWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// Handles the logic to be executed when the window is closing.
    /// Invokes the related cleanup logic in the associated view model and ensures
    /// proper disposal of resources.
    /// </summary>
    /// <param name="e">Provides data for the window closing event, including the ability to cancel the close.</param>
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        var viewModel = DataContext as StartWindowViewModel;
        viewModel?.OnClosing();
        base.OnClosing(e);
    }

    /// <summary>
    /// Invoked when the window has completed loading.
    /// </summary>
    /// <param name="e">
    /// Provides data related to the RoutedEvent that signals the completion of the loading process.
    /// </param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        if (DataContext is not StartWindowViewModel vm)
            return;
        _ = vm.OnLoaded();
        base.OnLoaded(e);
    }
}