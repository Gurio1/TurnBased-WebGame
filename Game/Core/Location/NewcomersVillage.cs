using Game.Core.Craft.Components;
using Game.Core.Loot;
using Game.Core.Marketplace;

namespace Game.Core.Location;

//TODO : Im not sure that i need specific class for every location. Create/Update endpoints will be enough i guess
public sealed class NewcomersVillage : Location
{
    public override string Name { get; protected set; } = "Newcomers' Village";
    public override string Description { get; protected set; } =
        "A quiet settlement nestled between whispering woods and mist-covered hills. " +
        "Newcomers’ Village welcomes weary travelers with the scent of fresh bread, " +
        "the chatter of merchants, and the distant clang of a blacksmith’s forge. " +
        "Though peaceful on the surface, the villagers speak of shadows stirring beyond the old stone bridge.";
    
    public override LootTable LootTable { get; protected set; } = new LootTable()
        .Add(new LootEntry {Chance = 40, Item = new Gold() , Quantity = 30})
        .Add(new LootEntry {Chance = 20, Item = new Iron() , Quantity = 3})
        .Add(new LootEntry {Chance = 20, Item = new Leather() , Quantity = 1})
        .Add(new LootEntry {Chance = 20, Item = new Stick() , Quantity = 4});
    protected override List<(double, LocationEvent)> Events { get; set; } = [(100, new LootDiscoveryEvent())];
}
