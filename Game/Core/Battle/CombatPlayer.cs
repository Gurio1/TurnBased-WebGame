using Game.Core.Models;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.ValueObjects;

namespace Game.Core.Battle;

public class CombatPlayer : CombatEntity
{
    public string? BattleId { get; set; }
    public List<InventorySlot> OtherInventoryItems { get; set; } = [];
    public Dictionary<string, int> UsedItems { get; set; } = new();
}

public static class Mapper
{
    public static CombatPlayer ToPlayerBattleModel(this Player model, Dictionary<string, int> playerUsedItems) =>
        new()
        {
            Id = model.Id,
            Name = model.CharacterType,
            BattleId = model.BattleId,
            Stats = model.Stats,
            Abilities = model.Abilities.ToArray(),
            Debuffs = model.Debuffs,
            CharacterType = model.CharacterType,
            Equipment = model.Equipment,
            OtherInventoryItems = model.Inventory.Where(i => i.Item.ItemType != ItemType.Equipment).ToList(),
            UsedItems = playerUsedItems
        };
}
