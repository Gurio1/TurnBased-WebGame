using Game.Core.Loot;
using Game.Core.PlayerProfile.Aggregates;
using Game.Utilities.Extensions;
using MongoDB.Bson;

namespace Game.Core.Location;

public abstract class Location
{
    public ObjectId Id { get; protected set; }
    public abstract string Name { get; protected set; }
    public abstract string Description { get; protected set; }
    public abstract LootTable LootTable { get; protected set; }
    protected abstract List<(double, LocationEvent)> Events { get; set; }

    public LootEntry? Explore(GamePlayer player)
    {
        double pick = RandomHelper.Instance.NextDouble() * Events.Sum(e => e.Item1);
        double acc = 0;

        foreach ((double chance, var ev) in Events)
        {
            acc += chance;
            if (!(pick <= acc)) continue;

            return ev.Trigger(player, this);
        }

        return null;
    }
}
