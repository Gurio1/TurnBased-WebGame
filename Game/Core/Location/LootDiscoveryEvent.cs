using Game.Core.Loot;
using Game.Core.PlayerProfile.Aggregates;

namespace Game.Core.Location;

public sealed class LootDiscoveryEvent : LocationEvent
{
    public string ImageUrl { get; set; } = "battle-background";
    public string Description { get; set; } = "Woow. You are so lucky, you found this materials for free on the road!";

    public override LootEntry? Trigger(GamePlayer player, Location location)
    {
        var loot = location.LootTable.GetRandomDrop();

        if (loot is null)
            return null;

        player.Inventory.Add(loot.Item, loot.Quantity);
        return loot;
    }
}
