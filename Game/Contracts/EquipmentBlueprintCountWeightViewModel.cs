namespace Game.Contracts;

public sealed record EquipmentBlueprintCountWeightViewModel
{
    public required int Count { get; init; }
    public required double Weight { get; init; }
}
