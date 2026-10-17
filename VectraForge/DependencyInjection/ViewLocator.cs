using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using VectraForge.ViewModel;

namespace VectraForge.DependencyInjection;

/// <summary>
/// A data template locator that resolves and creates views for the corresponding view models dynamically.
/// </summary>
/// <remarks>
/// This class implements the <see cref="Avalonia.Controls.Templates.IDataTemplate"/> interface to
/// map a ViewModel to its associated View. The naming convention assumes that the ViewModel
/// class name has "ViewModel" as a suffix, which will be replaced by "View" to derive the View's
/// class name at runtime.
/// 
/// If the corresponding view type is found, it is instantiated using the provided <see cref="IServiceProvider"/>.
/// Otherwise, a placeholder <see cref="Avalonia.Controls.TextBlock"/> is returned indicating that no view was found.
/// </remarks>
[ExcludeFromCodeCoverage]
public class ViewLocator(IServiceProvider serviceProvider) : IDataTemplate
{
    /// <summary>
    /// Builds a UI control corresponding to the provided ViewModel instance using the registered service provider.
    /// </summary>
    /// <param name="data">The ViewModel instance for which the corresponding View control is to be created.</param>
    /// <returns>
    /// A UI control associated with the ViewModel if resolved successfully; otherwise, a TextBlock indicating the View was not found.
    /// </returns>
    public Control? Build(object? data)
    {
        var viewModelType = data!.GetType();
        var viewTypeName = viewModelType.FullName!.Replace("ViewModel", "View");
        var viewType = Type.GetType(viewTypeName);

        if (viewType != null)
            return (Control)(serviceProvider.GetService(viewType)
                             ?? Activator.CreateInstance(viewType)!);
        
        return new TextBlock { Text = "Not Found:" + viewTypeName };
    }

    /// <summary>
    /// Determines whether the provided data object matches the expected type for this template.
    /// </summary>
    /// <param name="data">The data object to evaluate for compatibility with the template.</param>
    /// <returns>
    /// Returns <c>true</c> if the provided data object is of type <see cref="ViewModelBase"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool Match(object? data) => data is ViewModelBase;
}