namespace Game.Contracts;

public sealed record MonsterItemCatalogItemViewModel
{
    public required string TypeName { get; init; }
    public required string ItemId { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
    public required string Category { get; init; }
    public required bool IsEquipment { get; init; }
}
