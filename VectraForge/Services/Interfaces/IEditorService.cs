using VectraForge.ViewModel;

namespace VectraForge.Services.Interfaces;

public interface IEditorService
{
    event EventHandler<string>? OpenRequested;
    void RequestOpen(string path);
}