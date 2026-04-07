using Game.Battle.Core.Battle;

namespace Game.Battle.Core.Models;

public class Player : CombatEntity
{
    public string? BattleId { get; set; }
    public Inventory Inventory { get; set; } = new();
    public List<InventorySlot> OtherInventoryItems { get; set; } = [];
    public Dictionary<string, int> UsedItems { get; set; } = new();

    public bool InBattle() => BattleId is not null;
    public void ResetBattleId() => BattleId = null;
}
