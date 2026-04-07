using Game.Battle.Core.Models;

namespace Game.Battle.Core.Rewards;

public record BattleReward : IReward
{
    public int Gold { get; set; }
    public float Experience { get; set; }
    public List<Item>? Drop { get; set; }
    public List<Equipment>? EquipmentDrop { get; set; }
}
