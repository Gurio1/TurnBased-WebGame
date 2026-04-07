namespace Game.Contracts.Requests;

public sealed record MonsterDropEntryRequest
{
    public required string ItemTypeName { get; init; }
    public required string ItemId { get; init; }
    public required int Quantity { get; init; }
    public required double Weight { get; init; }
}
