using Microsoft.Extensions.DependencyInjection;
using VectraForge.ViewModel;
using VectraForge.Windows;

namespace VectraForge.DependencyInjection;

public static class VectraForgeServiceProviderExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddVectraForgeServices()
        {
            return services.AddWindows()
                .AddViewModels()
                .AddApplicationServices();
        }

        private IServiceCollection AddWindows()
        {
            services.AddSingleton<StartWindow>();
            return services;
        }

        private IServiceCollection AddViewModels()
        {
            services.AddSingleton<StartWindowViewModel>();
            return services;
        }

        private IServiceCollection AddApplicationServices()
        {
            return services;
        }
    }
}