namespace Game.Contracts;

public sealed record MonsterDropEntryViewModel
{
    public required string ItemTypeName { get; init; }
    public required string ItemId { get; init; }
    public required string ItemName { get; init; }
    public required string ItemImageUrl { get; init; }
    public required string Category { get; init; }
    public required int Quantity { get; init; }
    public required double Weight { get; init; }
}
