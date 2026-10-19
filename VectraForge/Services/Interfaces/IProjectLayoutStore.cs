using VectraForge.Models;

namespace VectraForge.Services.Interfaces;

public interface IProjectLayoutStore
{
    LayoutState? Load(ProjectContext context);
    void Save(ProjectContext context, LayoutState state);
}