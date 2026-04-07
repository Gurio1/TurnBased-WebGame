using Game.Battle.Core.Models;

namespace Game.Battle.Core.Rewards;

public interface IReward
{
    int Gold { get; set; }
    float Experience { get; set; }
    List<Item>? Drop { get; set; }
    List<Equipment>? EquipmentDrop { get; set; }
}
