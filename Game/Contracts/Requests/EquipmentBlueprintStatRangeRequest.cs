using System.ComponentModel.DataAnnotations;

namespace Game.Contracts.Requests;

public sealed record EquipmentBlueprintStatRangeRequest
{
    [Required]
    public required string StatKey { get; init; }

    public required float MinValue { get; init; }
    public required float MaxValue { get; init; }
}
