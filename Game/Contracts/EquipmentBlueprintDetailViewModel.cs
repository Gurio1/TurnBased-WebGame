namespace Game.Contracts;

public sealed record EquipmentBlueprintDetailViewModel
{
    public required string Id { get; init; }
    public required string EquipmentId { get; init; }
    public required string EquipmentName { get; init; }
    public required string EquipmentSlot { get; init; }
    public required string EquipmentImageUrl { get; init; }
    public required List<EquipmentBlueprintStatRangeViewModel> Stats { get; init; }
    public required List<EquipmentBlueprintCountWeightViewModel> CountWeights { get; init; }
}
