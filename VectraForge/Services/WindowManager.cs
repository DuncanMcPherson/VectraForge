using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using VectraForge.Services.Interfaces;
using VectraForge.Windows;

namespace VectraForge.Services;

/// <inheritdoc/>
public class WindowManager(IServiceProvider provider) : IWindowManager
{
    /// <inheritdoc/>
    public void ShowStartWindow()
    {
        var window = provider.GetRequiredService<StartWindow>();
        SwapMainWindow(window);
    }

    /// <inheritdoc/>
    public void ShowProjectWindow()
    {
        
    }

    /// <inheritdoc/>
    public Task<TResult?> ShowDialogWindow<TWindow, TResult>() where TWindow : Window
    {
        throw new NotImplementedException("Dialogs not supported, yet");
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