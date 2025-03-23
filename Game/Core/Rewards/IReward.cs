using Game.Core.Models;

namespace Game.Core.Rewards;

public interface IReward
{
    public int Gold { get; set; }
    public float Experience { get; set; }
    
    public Item? Drop { get; set; }
}