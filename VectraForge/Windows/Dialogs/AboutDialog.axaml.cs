using System.Diagnostics.CodeAnalysis;
using VectraForge.ViewModel;

namespace VectraForge.Windows.Dialogs;

[ExcludeFromCodeCoverage]
public partial class AboutDialog : Dialog
{
    public AboutDialog()
    {
        InitializeComponent();
    }

    public AboutDialog(AboutDialogViewModel viewModel)
    : this()
    {
        DataContext = viewModel;
    }
}