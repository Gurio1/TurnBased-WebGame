using System.ComponentModel.DataAnnotations;

namespace Game.Contracts.Requests;

public sealed record CreateEquipmentBlueprintRequest
{
    [Required]
    public required string EquipmentId { get; init; }

    [MinLength(1)]
    public required List<EquipmentBlueprintStatRangeRequest> Stats { get; init; }

    [MinLength(1)]
    public required List<EquipmentBlueprintCountWeightRequest> CountWeights { get; init; }
}
