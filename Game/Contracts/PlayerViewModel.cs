using Game.Core.PlayerProfile.Aggregates;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Core.StatusEffects;
using Game.Utilities;

namespace Game.Contracts;

public class PlayerViewModel
{
    public string Id { get; set; }
    public Stats Stats { get; set; }
    public List<AbilityHomeViewModel> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentViewModel?> Equipment { get; set; } = [];
    public List<InventorySlotViewModel> Inventory { get; set; } = new();
    public List<IDebuff> Debuffs { get; set; } = [];
    public string CharacterType { get; set; }
}
