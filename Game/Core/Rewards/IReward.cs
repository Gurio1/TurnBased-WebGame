using Game.Core.Equipment;
using Game.Core.Models;

namespace Game.Core.Rewards;

public interface IReward
{
    public int Gold { get; set; }
    public float Experience { get; set; }
    
    public List<Item>? Drop { get; set; }
    public List<EquipmentBase>? EquipmentDrop { get; set; }
}