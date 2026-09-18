using VectraForge.Models;
using VectraForge.Services;

namespace VectraForge;

public static class TypeExtensions
{
    extension(ProjectSourceKind kind)
    {
        public ProjectKind ToProjectKind()
        {
            return kind switch
            {
                ProjectSourceKind.Folder => ProjectKind.Folder,
                ProjectSourceKind.File => ProjectKind.File,
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind,
                    "Cancelled or unknown source kind cannot be converted to a ProjectKind"),
            };
        }
    }
}