using System.Diagnostics.CodeAnalysis;
using VectraForge.Models;
using VectraForge.Services;
using VectraForge.ViewModel;

namespace VectraForge.Windows.Dialogs;

[ExcludeFromCodeCoverage]
public partial class ProjectPickerDialog : Dialog
{
    public ProjectPickerDialog()
    {
        InitializeComponent();
        DataContext = new ProjectPickerDialogViewModel(new AvaloniaStoragePicker(StorageProvider));
    }

    protected override void OnEscapePressed()
    {
        Close(new ProjectSourceSelection(ProjectSourceKind.Cancelled, string.Empty));
    }
}