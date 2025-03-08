using Game.Drop;

namespace Game.Core.Rewards;

public interface IReward
{
    public int Gold { get; set; }
    public float Experience { get; set; }
    
    public IDropable? Drop { get; set; }
}