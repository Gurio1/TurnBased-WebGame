namespace Game.Battle.Core.Models;

public sealed class LootItem : Item
{
    public LootItem(int maxInventorySlotQuantity = 99) => MaxInventorySlotQuantity = maxInventorySlotQuantity;

    public override string Name { get; set; } = nameof(LootItem);
    public override string ImageUrl { get; init; } = string.Empty;
    public override int MaxInventorySlotQuantity { get; protected set; } = 99;
}
