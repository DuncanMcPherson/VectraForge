using System.Diagnostics.CodeAnalysis;
using VectraForge.Models;
using VectraForge.Services.Interfaces;

namespace VectraForge.Services;

public class ToolWindowRegistry : IToolWindowRegistry
{
    private readonly Dictionary<string, ToolWindow> _byId = new();
    public IReadOnlyList<ToolWindow> All { get; }

    public ToolWindowRegistry(IEnumerable<ToolWindow> tools)
    {
        All = [.. tools];
        foreach (var t in All)
            if (!_byId.TryAdd(t.Id, t))
                throw new InvalidOperationException($"Duplicate tool window id '{t.Id}'.");
    }

    public bool TryGet(string id, [MaybeNullWhen(false)] out ToolWindow tool) => _byId.TryGetValue(id, out tool);
}