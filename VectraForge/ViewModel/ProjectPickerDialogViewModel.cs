using System.Windows.Input;
using Avalonia.Platform.Storage;
using VectraForge.Commands;
using VectraForge.Models;
using VectraForge.Services.Interfaces;

namespace VectraForge.ViewModel;

public class ProjectPickerDialogViewModel : ViewModelBase, IRequestClose
{
    public event Action<object?>? CloseRequested;

    private readonly IProjectSourcePicker _projectSourcePicker;
    
    public ICommand? PickFolderCommand { get; private set; }
    public ICommand? PickFileCommand { get; private set; }
    
    public ProjectPickerDialogViewModel(IProjectSourcePicker projectSourcePicker)
    {
        _projectSourcePicker = projectSourcePicker;
        ConfigureCommands();
    }

    private void ConfigureCommands()
    {
        PickFolderCommand = new AsyncRelayCommand(async () =>
        {
            var selection = await PickAsync(_projectSourcePicker.PickFolderAsync, ProjectSourceKind.Folder);
            CloseRequested?.Invoke(selection);
        });
        
        PickFileCommand = new AsyncRelayCommand(async () =>
        {
            var selection = await PickAsync(_projectSourcePicker.PickFileAsync, ProjectSourceKind.File);
            CloseRequested?.Invoke(selection);
        });
    }

    private async Task<ProjectSourceSelection> PickAsync(Func<Task<IStorageItem?>> pick, ProjectSourceKind kind)
    {
        using var selectedItem = await pick();
        if (selectedItem is null)
        {
            return new ProjectSourceSelection(ProjectSourceKind.Cancelled, string.Empty);
        }
        
        var path = selectedItem.Path.LocalPath;
        return new ProjectSourceSelection(kind, path);
    }
}