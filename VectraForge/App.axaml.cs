using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using VectraForge.DependencyInjection;
using VectraForge.Services.Interfaces;

namespace VectraForge;

/// <summary>
/// The <see cref="App"/> class serves as the entry point for the Avalonia application.
/// It manages the initialization of the application, dependency injection configuration,
/// and framework-level configurations necessary for running the application.
/// </summary>
public partial class App : Application
{
    /// Initializes the application and loads the Avalonia XAML resources.
    /// 
    /// This method is responsible for loading the XAML markup for the application
    /// to define the visual layout and appearance. It is invoked during the application
    /// startup process to ensure that the UI resources are correctly loaded and ready for use.
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// Overrides the framework initialization completion method for the application.
    /// 
    /// This method is called once the framework has completed its initialization process.
    /// It sets up necessary services, resolves dependencies, and initializes the application's
    /// data templates and the main window.
    /// 
    /// The method performs the following steps:
    /// - Configures a service collection and registers necessary services.
    /// - Builds a service provider with appropriate options depending on the build configuration
    /// (e.g., validating scopes and builds in debug mode).
    /// - Adds a custom view locator for resolving views and view models at runtime.
    /// - Retrieves and uses an instance of IWindowManager to display the start window.
    /// 
    /// This method ensures that the application's dependency injection system and main window
    /// are initialized properly before the application begins execution.
    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IServiceProvider>(sp => sp);

        var provider = services.AddVectraForgeServices()
#if DEBUG
            .BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true,
            });
#else
        .BuildServiceProvider();
#endif

        DataTemplates.Add(new ViewLocator(provider));

        var windowManager = provider.GetRequiredService<IWindowManager>();
        windowManager.ShowStartWindow();

        base.OnFrameworkInitializationCompleted();
    }
}