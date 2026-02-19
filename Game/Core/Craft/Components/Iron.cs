using Game.Core.Marketplace;

namespace Game.Core.Craft.Components;

public class Iron : CraftingComponent
{
    public override string Id { get; set; } = "iron";
    public override string Name { get; set; } = "Iron";
    public override Currency SellPrice { get; set; } = new Gold() { Quantity = 100 };
    public override string ImageUrl { get; init; } = "iron";
    public override int MaxInventorySlotQuantity { get; protected set; } = 20;
}
