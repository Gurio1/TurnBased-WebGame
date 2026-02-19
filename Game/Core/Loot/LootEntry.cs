using Game.Core.Models;

namespace Game.Core.Loot;

public record LootEntry
{
    public required double Chance { get; init; }
    public required Item Item { get; init; }
    public required int Quantity { get; init; }
}
