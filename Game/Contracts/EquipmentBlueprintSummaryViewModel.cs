namespace Game.Contracts;

public sealed record EquipmentBlueprintSummaryViewModel
{
    public required string Id { get; init; }
    public required string EquipmentId { get; init; }
    public required string EquipmentName { get; init; }
    public required string EquipmentSlot { get; init; }
    public required string EquipmentImageUrl { get; init; }
    public required int StatCount { get; init; }
    public required int MaxRollCount { get; init; }
}
