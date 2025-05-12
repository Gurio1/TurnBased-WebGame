namespace Game.Core.Equipment.Generation;

public sealed class EquipmentGenerationContext(EquipmentBase equipment)
{
    public EquipmentBase Equipment { get; init; } = equipment;
    public BlueprintAttributes? Blueprint { get; init; }
}
