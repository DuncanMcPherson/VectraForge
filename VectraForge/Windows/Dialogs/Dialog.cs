using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Input;
using VectraForge.Models;

namespace VectraForge.Windows.Dialogs;

[ExcludeFromCodeCoverage]
public abstract class Dialog : Window
{
    protected Dialog()
    {
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e is { Key: Key.Escape, Handled: false })
        {
            OnEscapePressed();
            e.Handled = true;
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is IRequestClose requestClose)
        {
            requestClose.CloseRequested += Close;
        }
    }

    protected virtual void OnEscapePressed() => Close();
}