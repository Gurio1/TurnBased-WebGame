using Game.Core.Marketplace;

namespace Game.Core.Craft.Components;

public sealed class Stick : CraftingComponent
{
    public override string Id { get; set; } = "stick";
    public override string Name { get; set; } = "Stick";
    public override Currency SellPrice { get; set; } = new Gold() { Quantity = 100 };
    public override string ImageUrl { get; init; } = "stick";
    public override int MaxInventorySlotQuantity { get; protected set; } = 50;
}
