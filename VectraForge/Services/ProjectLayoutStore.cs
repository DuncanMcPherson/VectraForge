using System.Text.Json;
using VectraForge.Models;
using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

public class ProjectLayoutStore : IProjectLayoutStore
{
    private readonly IAtomicFileStore _fileStore;

    public ProjectLayoutStore(IAtomicFileStore fileStore)
    {
        _fileStore = fileStore;
    }

    public LayoutState? Load(ProjectContext context)
    {
        try
        {
            var layoutFilePath = LayoutPath(context);

            var fileContents = _fileStore.Read(layoutFilePath);
            return fileContents is null ? null : LayoutState.FromJson(fileContents);
        }
        catch (Exception e) when (e is JsonException or IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    public void Save(ProjectContext context, LayoutState state)
    {
        try
        {
            var layoutFilePath = LayoutPath(context);
            var ignorePath = Path.Combine(Path.GetDirectoryName(layoutFilePath)!, ".gitignore");
            if (_fileStore.Read(ignorePath) is null)
                _fileStore.Write(ignorePath, "*");
            var fileContents = state.ToJson();
            _fileStore.Write(layoutFilePath, fileContents);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            // Ignore
        }
    }

    private string LayoutPath(ProjectContext context)
    {
        return context.Kind switch
        {
            ProjectKind.Folder => Path.Combine(context.RootPath, ".vf", "layout.json"),
            ProjectKind.File => Path.Combine(Path.GetDirectoryName(context.RootPath)!, ".vf", "layout.json"),
            _ => throw new IndexOutOfRangeException($"Unknown project kind: {context.Kind}")
        };
    }
}