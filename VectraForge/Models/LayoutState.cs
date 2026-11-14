using System.Text.Json;
using System.Text.Json.Serialization;

namespace VectraForge.Models;

public sealed record LayoutState
{
    public const int CurrentVersion = 1;
    public int Version { get; init; } = CurrentVersion;
    public IReadOnlyDictionary<WindowSlot, SlotState> Slots { get; init; } = new Dictionary<WindowSlot, SlotState>();

    public static double DefaultSize(WindowSlot slot) => slot switch
    {
        WindowSlot.Left => 260,
        WindowSlot.Bottom => 300,
        WindowSlot.Right => 220,
        _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
    };
    
    public static double MinSize(WindowSlot slot) => slot switch
    {
        WindowSlot.Left => 100,
        WindowSlot.Bottom => 150,
        WindowSlot.Right => 100,
        _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
    };

    public static double MaxSize(WindowSlot slot) => slot switch
    {
        WindowSlot.Left => 1000,
        WindowSlot.Bottom => 1200,
        WindowSlot.Right => 1000,
        _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
    };

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static LayoutState? FromJson(string json) => JsonSerializer.Deserialize<LayoutState>(json, Options);
    public string ToJson() => JsonSerializer.Serialize(this, Options);

    public static LayoutState Reconcile(LayoutState? saved, IEnumerable<ToolWindow> registered)
    {
        var tools = registered.ToList();
        var known = tools.Select(t => t.Id).ToHashSet();
        var placed = new HashSet<string>();
        var slots = Enum.GetValues<WindowSlot>();
        var ids = slots.ToDictionary(s => s, _ => new List<string>());

        foreach (var slot in slots)
        {
            foreach (var id in SavedSlot(saved, slot)?.ToolIds ?? [])
            {
                if (known.Contains(id) && placed.Add(id))
                    ids[slot].Add(id);
            }
        }
        
        foreach (var tool in tools.Where(tool => placed.Add(tool.Id)))
            ids[tool.DefaultSlot].Add(tool.Id);

        var result = new Dictionary<WindowSlot, SlotState>();
        foreach (var slot in slots)
        {
            var s = SavedSlot(saved, slot);
            var size = s is not null && double.IsFinite(s.Size) && s.Size > 0
                ? Math.Clamp(s.Size, MinSize(slot), MaxSize(slot))
                : DefaultSize(slot);
            var active = s?.ActiveToolId is { } a && ids[slot].Contains(a) ? a : ids[slot].FirstOrDefault();
            result[slot] = new SlotState
            {
                Size = size, IsCollapsed = s?.IsCollapsed ?? false, ActiveToolId = active, ToolIds = ids[slot]
            };
        }

        return new LayoutState
        {
            Slots = result
        };
    }

    private static SlotState? SavedSlot(LayoutState? saved, WindowSlot slot) =>
        saved?.Slots is { } s && s.TryGetValue(slot, out var v) ? v : null;
}

public sealed record SlotState
{
    public double Size { get; init; }
    public bool IsCollapsed { get; init; }
    public string? ActiveToolId { get; init; }
    public IReadOnlyList<string> ToolIds { get; init; } = [];
}