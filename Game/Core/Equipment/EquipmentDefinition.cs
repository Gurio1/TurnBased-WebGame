namespace Game.Core.Equipment;

public sealed record EquipmentDefinition(
    string TypeName,
    string EquipmentId,
    string Name,
    string Slot,
    string ImageUrl);
