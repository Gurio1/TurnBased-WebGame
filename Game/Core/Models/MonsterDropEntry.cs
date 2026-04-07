namespace Game.Core.Models;

public sealed class MonsterDropEntry
{
    public required string ItemTypeName { get; init; }
    public required string ItemId { get; init; }
    public int Quantity { get; init; }
    public double Weight { get; init; }
}
