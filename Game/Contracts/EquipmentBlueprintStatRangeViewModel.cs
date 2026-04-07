namespace Game.Contracts;

public sealed record EquipmentBlueprintStatRangeViewModel
{
    public required string StatKey { get; init; }
    public required string StatName { get; init; }
    public required float MinValue { get; init; }
    public required float MaxValue { get; init; }
}
