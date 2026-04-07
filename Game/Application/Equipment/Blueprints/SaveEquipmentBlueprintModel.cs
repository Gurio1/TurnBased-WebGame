using Game.Core.Equipment;

namespace Game.Application.Equipment.Blueprints;

public sealed record SaveEquipmentBlueprintModel
{
    public required string EquipmentId { get; init; }
    public required IReadOnlyCollection<BlueprintStatRange> Stats { get; init; }
    public required IReadOnlyCollection<KeyValuePair<int, double>> CountWeights { get; init; }
}
