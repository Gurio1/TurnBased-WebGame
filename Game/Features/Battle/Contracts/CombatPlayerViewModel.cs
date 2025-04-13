using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Features.Battle.Models;

namespace Game.Features.Battle.Contracts;

public class CombatPlayerViewModel
{
    public string Id { get; set; }
    public Stats Stats { get; set; }
    public List<Ability> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; set; } = [];
    public List<InventorySlot> OtherInventoryItems { get; set; } = new();
    public List<IDebuff> Debuffs { get; set; } = [];
    public string CharacterType { get; set; }
}

public static partial class Mapper
{
    public static CombatPlayerViewModel ToViewModel(this CombatPlayer model) =>
        new()
        {
            Id = model.Id,
            Stats = model.Stats,
            Abilities = model.Abilities,
            Debuffs = model.Debuffs,
            CharacterType = model.CharacterType,
            Equipment = model.Equipment,
            OtherInventoryItems = model.OtherInventoryItems
        };
}
