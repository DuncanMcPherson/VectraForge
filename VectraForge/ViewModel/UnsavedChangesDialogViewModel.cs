using System.Windows.Input;
using VectraForge.Commands;
using VectraForge.Models;

namespace VectraForge.ViewModel;

public enum SaveChoice {Cancel = 0, Save, DontSave}

public class UnsavedChangesDialogViewModel : ViewModelBase, IRequestClose
{
    public event Action<object?>? CloseRequested;
    public string Prompt { get; }
    public ICommand SaveCommand { get; }
    public ICommand DontSaveCommand { get; }
    public ICommand CancelCommand { get; }

    public UnsavedChangesDialogViewModel(IReadOnlyList<string> names)
    {
        Prompt = names.Count == 1
            ? $"Save changes to {names[0]}?"
            : $"Save changes to {names.Count} files?\n{string.Join('\n', names)}";
        SaveCommand = new RelayCommand(() => CloseRequested?.Invoke(SaveChoice.Save));
        DontSaveCommand = new RelayCommand(() => CloseRequested?.Invoke(SaveChoice.DontSave));
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(SaveChoice.Cancel));
    }
}