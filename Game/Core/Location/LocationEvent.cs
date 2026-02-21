using Game.Core.Loot;
using Game.Core.PlayerProfile.Aggregates;

namespace Game.Core.Location;

public abstract class LocationEvent
{
    public abstract LootEntry? Trigger(GamePlayer player, Location location);
}
