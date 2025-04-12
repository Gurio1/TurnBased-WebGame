using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Features.Battle.Contracts;

namespace Game.Features.Battle.Models;

public class CombatPlayer : CombatEntity
{
    public string? BattleId { get; set; }
    public List<Ability> Abilities { get; set; } = [];
    public List<InventorySlot> OtherInventoryItems { get; set; } = [];
    public Dictionary<string,int> UsedItems { get; set; } = new();
}

public static partial class  Mapper
{
    public static CombatPlayer ToPlayerBattleModel(this Player model, Dictionary<string, int> playerUsedItems)
    {
        return new CombatPlayer()
        {
            Id = model.Id,
            Name = model.CharacterType,
            BattleId = model.BattleId,
            Stats = model.Stats,
            Abilities = model.Abilities,
            Debuffs = model.Debuffs,
            CharacterType = model.CharacterType,
            Equipment = model.Equipment,
            OtherInventoryItems = model.Inventory.Where(i => i.Item.ItemType != ItemType.Equipment).ToList(),
            UsedItems = playerUsedItems
        };
    }
}
