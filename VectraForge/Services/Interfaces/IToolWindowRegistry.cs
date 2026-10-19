using System.Diagnostics.CodeAnalysis;
using VectraForge.Models;

namespace VectraForge.Services.Interfaces;

public interface IToolWindowRegistry
{
    IReadOnlyList<ToolWindow> All { get; }
    bool TryGet(string id, [MaybeNullWhen(false)] out ToolWindow tool);
}