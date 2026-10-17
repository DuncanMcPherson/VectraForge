using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using VectraForge.Services.Interfaces;
using VectraForge.ViewModel;
using VectraForge.Windows;
using VectraForge.Windows.Dialogs;

namespace VectraForge.Services;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
public class WindowManager(IServiceProvider provider, IProjectContextAccessor contextAccessor) : IWindowManager
{
    private readonly Dictionary<ViewModelBase, Window> _dialogOwners = new();
    
    /// <inheritdoc/>
    public void ShowStartWindow()
    {
        var window = provider.GetRequiredService<StartWindow>();
        SwapMainWindow(window);
    }

    /// <inheritdoc/>
    public void ShowProjectWindow(ProjectContext? context)
    {
        contextAccessor.Current = context;
        var window = provider.GetRequiredService<MainWindow>();
        SwapMainWindow(window);
    }

    /// <inheritdoc/>
    public Task<TResult?> ShowDialogWindow<TWindow, TResult>(ViewModelBase owningContext) where TWindow : Dialog
    {
        Task<TResult?> OpenDialog()
        {
            var owner = _dialogOwners.FirstOrDefault(kvp => kvp.Key == owningContext).Value;
            if (owner is null)
                throw new InvalidOperationException("No owner window registered for the dialog.");
            var dialog = provider.GetRequiredService<TWindow>();
            return dialog.ShowDialog<TResult?>(owner);
        }
        return Dispatcher.UIThread.CheckAccess() ? OpenDialog() : Dispatcher.UIThread.Invoke(OpenDialog);
    }

    /// <inheritdoc/>
    public void RegisterDialogOwner(ViewModelBase owningContext, Window owningWindow)
    {
        _dialogOwners[owningContext] = owningWindow;
    }
    
    /// <inheritdoc/>
    public void UnregisterDialogOwner(ViewModelBase owningContext)
    {
        _dialogOwners.Remove(owningContext);
    }

    /// <summary>
    /// Replaces the current main window of the application with the specified window.
    /// The newly set window is shown immediately, and the previous main window is closed if it exists.
    /// </summary>
    /// <param name="window">The new window to set as the main application window. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="window"/> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the application instance is not initialized or is not running with a desktop application lifetime.
    /// </exception>
    private static void SwapMainWindow(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        var app = Application.Current ?? throw new InvalidOperationException(
            "Application.Current is null - WindowManager was used before Avalonia finished initializing.");
        if (app.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            throw new InvalidOperationException(
                $"WindowManager requires a desktop application lifetime, but got '{app.ApplicationLifetime?.GetType().Name ?? "null"}'.");
        var old = desktop.MainWindow;
        desktop.MainWindow = window;
        window.Show();
        old?.Close();
    }
}