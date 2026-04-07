namespace Game.Battle.Core.Models;

public sealed class Equipment : Item
{
    public string EquipmentId { get; set; } = Guid.NewGuid().ToString();
    public override string Name { get; set; } = nameof(Equipment);
    public override string ImageUrl { get; init; } = string.Empty;
    public override int MaxInventorySlotQuantity { get; protected set; } = 1;
    public string Slot { get; set; } = "loot";
}
