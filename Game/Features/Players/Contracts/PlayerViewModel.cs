using Game.Core.Equipment;
using Game.Core.Models;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Core.StatusEffects;
using Game.Features.Abilities.Contracts;

namespace Game.Features.Players.Contracts;

public class PlayerViewModel
{
    public string Id { get; set; }
    public Stats Stats { get; set; }
    public List<AbilityHomeViewModel> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; set; } = [];
    public List<EquipmentBase> InventoryEquipmentItems { get; set; } = new();
    public List<InventorySlot> OtherInventoryItems { get; set; } = new();
    public List<IDebuff> Debuffs { get; set; } = [];
    public string CharacterType { get; set; }
}

public static class Mapper
{
    public static PlayerViewModel ToViewModel(this Player model) =>
        new()
        {
            Id = model.Id,
            Stats = model.Stats,
            Abilities = model.Abilities.Select(a => a.ToAbilityHomeViewModel(model)).ToList(),
            Debuffs = model.Debuffs,
            CharacterType = model.CharacterType,
            Equipment = model.Equipment,
            InventoryEquipmentItems =
                model.Inventory.Select(s => s.Item).Where(i => i.ItemType == ItemType.Equipment)
                    .Cast<EquipmentBase>().ToList(),
            OtherInventoryItems = model.Inventory.Where(i => i.Item.ItemType != ItemType.Equipment).ToList()
        };
}
