using System.Threading.Tasks;
using Avalonia.Controls;
using VectraForge.ViewModel;
using VectraForge.Windows;
using VectraForge.Windows.Dialogs;

namespace VectraForge.Services.Interfaces;

/// <summary>
/// Provides methods for managing and displaying application windows.
/// </summary>
public interface IWindowManager
{
    /// <summary>
    /// Displays the application start window as the main window.
    /// This method retrieves the <see cref="StartWindow"/> instance from the service provider
    /// and swaps it into the main window position of the application.
    /// </summary>
    void ShowStartWindow();
    
    /// <summary>
    /// Displays the main application window for working on a project.
    /// This method is responsible for initializing and showing the primary interface
    /// where users can create, edit, or manage their project context.
    /// </summary>
    /// <remarks>
    /// The method's implementation depends on proper integration with a ProjectContext
    /// and the ProjectWindow. These dependencies should be resolved to ensure proper
    /// functionality.
    /// </remarks>
    /// <exception cref="NotImplementedException">
    /// Thrown if the method is called before the required ProjectContext and ProjectWindow
    /// are implemented and integrated.
    /// </exception>
    void ShowProjectWindow(ProjectContext? context);

    /// <summary>
    /// Displays a dialog window of the specified type and returns a result asynchronously.
    /// </summary>
    /// <typeparam name="TWindow">
    /// The type of the dialog window to display, which must inherit from <see cref="Dialog"/>.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result expected from the dialog window.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation of displaying the dialog window.
    /// The task's result contains the value of type <typeparamref name="TResult"/> returned by the dialog window,
    /// or <c>null</c> if no result is provided.
    /// </returns>
    Task<TResult?> ShowDialogWindow<TWindow, TResult>(ViewModelBase owningContext) where TWindow : Dialog;

    /// <summary>
    /// Registers a window to it's associated view model to handle dialog creation
    /// </summary>
    /// <param name="owningContext">
    /// The view model that will call for dialogs
    /// </param>
    /// <param name="owningWindow">
    /// The <see cref="Window"/> that owns dialogs created from the view model
    /// </param>
    void RegisterDialogOwner(ViewModelBase owningContext, Window owningWindow);

    /// <summary>
    /// Unregisters a window from dialog ownership
    /// </summary>
    /// <param name="owner">
    /// The <see cref="Window"/> that owned dialogs
    /// </param>
    void UnregisterDialogOwner(ViewModelBase owner);
}