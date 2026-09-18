namespace VectraForge.Services.Interfaces;

public interface IProjectContextAccessor
{
    ProjectContext? Current { get; set; }
}