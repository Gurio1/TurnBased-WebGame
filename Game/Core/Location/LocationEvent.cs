using Game.Core.Models;
using Game.Core.PlayerProfile.Aggregates;

namespace Game.Core.Location;

public abstract class LocationEvent
{
    public double Weight { get; private set; }
    public abstract Item? Trigger(GamePlayer player,Location location);
}
