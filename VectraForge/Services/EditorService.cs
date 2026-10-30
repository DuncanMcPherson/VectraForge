using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

public class EditorService : IEditorService
{
    public event EventHandler<string>? OpenRequested;
    
    public void RequestOpen(string path)
    {
        OpenRequested?.Invoke(this, path);
    }
}