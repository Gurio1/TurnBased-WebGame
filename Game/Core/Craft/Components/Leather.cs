using Game.Core.Marketplace;

namespace Game.Core.Craft.Components;

public sealed class Leather : CraftingComponent
{
    public override string Id { get; set; } = "leather";
    public override string Name { get; set; } = "Leather";
    public override Currency SellPrice { get; set; } = new Gold() { Quantity = 100 };
    public override string ImageUrl { get; init; } = "leather";
    public override int MaxInventorySlotQuantity { get; protected set; } = 6;
}
