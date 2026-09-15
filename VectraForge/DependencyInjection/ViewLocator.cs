using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using VectraForge.ViewModel;

namespace VectraForge.DependencyInjection;

public class ViewLocator : IDataTemplate
{
    private readonly IServiceProvider _serviceProvider;
    
    public ViewLocator(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Control? Build(object? data)
    {
        var viewModelType = data!.GetType();
        var viewTypeName = viewModelType.FullName!.Replace("ViewModel", "View");
        var viewType = Type.GetType(viewTypeName);

        if (viewType != null)
            return (Control)(_serviceProvider.GetService(viewType)
                             ?? Activator.CreateInstance(viewType)!);
        
        return new TextBlock { Text = "Not Found:" + viewTypeName };
    }

    public bool Match(object? data) => data is ViewModelBase;
}