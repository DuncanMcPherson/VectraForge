using Avalonia.Platform.Storage;

namespace VectraForge.Services.Interfaces;

public interface IProjectSourcePicker
{
    Task<IStorageItem?> PickFolderAsync();
    Task<IStorageItem?> PickFileAsync();
}