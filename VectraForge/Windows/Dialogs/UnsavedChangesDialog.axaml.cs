using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VectraForge.ViewModel;

namespace VectraForge.Windows.Dialogs;

public partial class UnsavedChangesDialog : Dialog
{
    public UnsavedChangesDialog()
    {
        InitializeComponent();
    }

    protected override void OnEscapePressed()
    {
        Close(SaveChoice.Cancel);
    }
}