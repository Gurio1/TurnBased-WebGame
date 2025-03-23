using System.Globalization;
using Game.Core.Models;

namespace Game.Core.Equipment.Boots;

public class WoodenBoots : EquipmentBase
{
    public override string Id { get; set; }

    public override string Slot { get; set; } = "Boots";
    
    public override string Name { get; set; } = "Wooden Boots";

}