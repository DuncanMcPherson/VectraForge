using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Avalonia.Threading;

namespace VectraForge.ViewModel;

/// <summary>
/// Serves as a base class for all view models in the application.
/// Provides common functionality such as property change notification, enabling support for data binding.
/// </summary>
/// <remarks>
/// This class implements the <see cref="INotifyPropertyChanged"/> interface to facilitate property change notification.
/// It also includes utility methods for property management that reduce boilerplate code in derived classes.
/// </remarks>
[ExcludeFromCodeCoverage]
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <remarks>
    /// This event is part of the implementation of the <see cref="INotifyPropertyChanged"/> interface.
    /// It is typically raised by calling the <c>OnPropertyChanged</c> method when a property's value
    /// has been modified to notify any observers that the value has changed.
    /// </remarks>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event to notify listeners about a change to a property value.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that changed. The default value is the name of the caller if no argument is provided.
    /// </param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        else
        {
            Dispatcher.UIThread.Post(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }
    }

    /// <summary>
    /// Sets the specified field to a new value and raises the PropertyChanged event
    /// if the value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the field and value.</typeparam>
    /// <param name="field">A reference to the field to set.</param>
    /// <param name="value">The new value to assign to the field.</param>
    /// <param name="propertyName">
    /// The name of the property that has changed. This is optional and is automatically populated
    /// by the caller's name using the CallerMemberName attribute if not specified.
    /// </param>
    /// <returns>
    /// True if the value was changed and the PropertyChanged event was raised;
    /// otherwise, false if the value is equal to the current field value.
    /// </returns>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}