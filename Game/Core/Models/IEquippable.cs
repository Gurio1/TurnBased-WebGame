using Game.Core.PlayerProfile.Aggregates;

namespace Game.Core.Models;

public interface IEquippable
{
    // bool CanBeEquippedBy(Player player);
    public void ApplyStats(GamePlayer characterBase);
    public void RemoveStats(GamePlayer characterBase);
}
