using Game.Features.Attributes;

namespace Game.Core.Equipment;

public class AttributeRange
{
    public EquipmentStat Stat { get; set; } 
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
}
