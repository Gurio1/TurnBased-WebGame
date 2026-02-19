using Game.Core.Abilities;
using Game.Core.Battle;
using Game.Core.Equipment;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Core.StatusEffects;

namespace Game.Contracts;

public class CombatPlayerViewModel
{
    public string Id { get; set; }
    public Stats Stats { get; set; }
    public Ability[] Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; set; } = [];
    public List<InventorySlot> OtherInventoryItems { get; set; } = new();
    public List<IDebuff> Debuffs { get; set; } = [];
    public string CharacterType { get; set; }
}
