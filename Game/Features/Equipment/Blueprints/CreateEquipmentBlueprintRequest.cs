using System.ComponentModel.DataAnnotations;

namespace Game.Features.Equipment.Blueprints.Create;

public sealed class CreateEquipmentBlueprintRequest
{
    [Required]
    public required string EquipmentId { get; init; }
}
