namespace Game.Core.Equipment.Weapons;

public class WoodenSword : EquipmentBase
{
    public override string EquipmentId { get; set; } = "wooden-sword";
    
    public override string Slot { get; set; } = "Weapon";
    
    public override string Name { get; set; } = "Wooden sword";
    
    public override string ImageUrl { get; set; } = "wooden-sword";
}
