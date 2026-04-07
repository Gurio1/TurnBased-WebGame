namespace Game.Contracts;

public sealed record EquipmentBlueprintEquipmentCatalogItemViewModel
{
    public required string TypeName { get; init; }
    public required string EquipmentId { get; init; }
    public required string Name { get; init; }
    public required string Slot { get; init; }
    public required string ImageUrl { get; init; }
    public string? AssignedBlueprintId { get; init; }
}
