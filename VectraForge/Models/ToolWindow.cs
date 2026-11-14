using Dock.Model.Core;
using VectraForge.Services;
using VectraForge.ViewModel;

namespace VectraForge.Models;

public enum WindowSlot
{
    Left,
    Bottom,
    Right
}

public record ToolWindow(
    string Id,
    string Title,
    WindowSlot DefaultSlot,
    Func<ProjectContext?, ViewModelBase> CreateView);
    