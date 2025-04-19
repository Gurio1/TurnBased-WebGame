namespace Game.Core.Equipment.Chests;

public class WoodenArmour : EquipmentBase
{
    public override string EquipmentId { get; set; } = "wooden-chest";
    
    public override string Slot { get; set; } = "Chest";
    
    public override string Name { get; set; } = "Wooden Armour";
    
    public override string ImageUrl { get; set; } = "wooden-chest";
}
