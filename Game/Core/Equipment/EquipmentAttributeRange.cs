using Game.Features.Attributes;

namespace Game.Core.Equipment;

public class EquipmentAttributeRange
{
    public EquipmentAttribute Attribute { get; set; } 
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
}