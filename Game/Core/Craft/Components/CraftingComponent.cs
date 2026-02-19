using Game.Core.Marketplace;
using Game.Core.Models;

namespace Game.Core.Craft.Components;

public abstract class CraftingComponent : Item, ISellable
{ 
    public abstract Currency SellPrice { get; set; }
}
