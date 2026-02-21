namespace Game.Features.Locations.Explore;

public sealed record ExploreResponse
{
    public required string LocationName { get; init; }
    public required string Message { get; init; }
    public string? ItemName { get; init; }
    public string? ItemImageUrl { get; init; }
    public int Quantity { get; init; }
}
