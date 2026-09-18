using Avalonia.Platform.Storage;
using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

public class AvaloniaStoragePicker : IProjectSourcePicker
{
    private readonly IStorageProvider _storageProvider;

    public AvaloniaStoragePicker(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<IStorageItem?> PickFolderAsync()
    {
        var results = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false
        });
        return results.FirstOrDefault();
    }
    
    public async Task<IStorageItem?> PickFileAsync()
    {
        var results = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = [
                new FilePickerFileType("Vectra Packages") { Patterns = ["*.vpkg"]},
                new FilePickerFileType("Vectra Modules") { Patterns = ["*.vmod"]}
            ]
        });
        return results.FirstOrDefault();
    }
}