using System.ComponentModel.DataAnnotations;

namespace Game.Contracts.Requests;

public sealed record EquipmentBlueprintCountWeightRequest
{
    [Range(1, int.MaxValue)]
    public required int Count { get; init; }

    [Range(0, double.MaxValue)]
    public required double Weight { get; init; }
}
