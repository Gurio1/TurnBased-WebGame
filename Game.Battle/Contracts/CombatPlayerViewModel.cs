using Game.Battle.Core.Abilities;
using Game.Battle.Core.Models;
using Game.Core.StatusEffects;
using Game.SharedKernel.Models;

namespace Game.Battle.Contracts;

public class CombatPlayerViewModel
{
    public string Id { get; set; }
    public Stats Stats { get; set; }
    public Ability[] Abilities { get; set; }
    public Dictionary<string, Equipment?> Equipment { get; set; }
    public List<InventorySlot> OtherInventoryItems { get; set; }
    public List<IDebuff> Debuffs { get; set; }
    public string CharacterType { get; set; }

    public CombatPlayerViewModel(Player model)
    {
        Id = model.Id;
        Stats = model.Stats;
        Abilities = model.Abilities;
        Debuffs = model.Debuffs;
        CharacterType = model.CharacterType;
        Equipment = model.Equipment;
        OtherInventoryItems = model.OtherInventoryItems;
    }
}
