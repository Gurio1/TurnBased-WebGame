namespace Game.Core.Equipment.Boots;

public class WoodenBoots : EquipmentBase
{
    public override string EquipmentId { get; set; } = "wooden-boots";
    public override string Slot { get; set; } = "Boots";
    public override string Name { get; set; } = "Wooden Boots";
    public override string ImageUrl { get; init; } = "wooden-boots";
}
