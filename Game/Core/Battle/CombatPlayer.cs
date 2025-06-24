using Game.Core.Models;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.ValueObjects;

namespace Game.Core.Battle;

public class CombatPlayer : CombatEntity
{
    public string BattleId { get; set; } = null!;
    public List<InventorySlot> OtherInventoryItems { get; set; } = [];
    public Dictionary<string, int> UsedItems { get; set; } = new();
}
