using Microsoft.Extensions.DependencyInjection;
using VectraForge.Models;
using VectraForge.Services;
using VectraForge.Services.Interfaces;
using VectraForge.ViewModel;
using VectraForge.Windows;
using VectraForge.Windows.Dialogs;

namespace VectraForge.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring and resolving dependencies
/// using the service provider in the context of VectraForge.
/// </summary>
public static class VectraForgeServiceProviderExtensions
{
    /// <summary>
    /// Provides extension methods for configuring VectraForge-related services for dependency injection.
    /// </summary>
    extension(IServiceCollection services)
    {
        /// Adds all core VectraForge services, view models, and windows to the service collection.
        /// 
        /// The method registers the following:
        /// - Core application services such as IWindowManager, IRecentProjectService, and IConfigService.
        /// - UI windows such as StartWindow.
        /// - View models such as StartWindowViewModel.
        /// 
        /// <returns>
        /// The same instance of IServiceCollection with VectraForge services added.
        /// </returns>
        public IServiceCollection AddVectraForgeServices()
        {
            return services.AddWindows()
                .AddViewModels()
                .AddApplicationServices()
                .AddToolWindows();
        }

        /// <summary>
        /// Registers the necessary services to enable window-related functionalities within the application.
        /// </summary>
        /// <returns>
        /// The <see cref="IServiceCollection"/> instance with window services added to the service container.
        /// </returns>
        private IServiceCollection AddWindows()
        {
            services.AddTransient<StartWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<ProjectPickerDialog>();
            services.AddTransient<AboutDialog>();
            services.AddTransient<UnsavedChangesDialog>();
            return services;
        }

        /// Adds ViewModel registrations to the provided IServiceCollection.
        /// This method registers all ViewModel dependencies as transient services,
        /// enabling dependency injection for ViewModel components within the application.
        /// 
        /// <returns>
        /// The IServiceCollection instance, allowing for fluent chaining of
        /// service registration methods.
        /// </returns>
        private IServiceCollection AddViewModels()
        {
            services.AddTransient<StartWindowViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<AboutDialogViewModel>();
            return services;
        }

        /// <summary>
        /// Registers core application services to the provided service collection.
        /// </summary>
        /// <returns>
        /// The updated IServiceCollection instance, allowing for chaining additional service configurations.
        /// </returns>
        private IServiceCollection AddApplicationServices()
        {
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<IRecentProjectService, RecentProjectService>();
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IProjectContextAccessor, ProjectContextAccessor>();
            services.AddSingleton<IProjectLayoutStore, ProjectLayoutStore>();
            services.AddSingleton<IAtomicFileStore, AtomicFileStore>();
            services.AddSingleton<IToolWindowRegistry, ToolWindowRegistry>();
            services.AddSingleton<IEditorService, EditorService>();
            return services;
        }

        private IServiceCollection AddToolWindows()
        {
            services.AddSingleton(sp =>
            {
                var editor = sp.GetRequiredService<IEditorService>();
                return new ToolWindow("core.files", "Files", WindowSlot.Left,
                    ctx => new FileTreeViewModel(ctx, editor));
            });
            return services;
        }
    }
}