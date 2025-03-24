using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Core.Models;
using MongoDB.Bson;

namespace Game.Features.Players.Contracts;

public class PlayerViewModel
{
    public string Id { get; set; }
    public float Hp { get; set; }
    public float Armor { get; set; }
    public float Damage { get; set; }
    public float DebuffResistance { get; set; }
    public float CriticalChance { get; set; }
    public float CriticalDamage { get; set; }
    public float DodgeChance { get; set; }
    public List<Ability> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; set; } = [];
    public List<EquipmentBase> InventoryEquipmentItems { get; set; } = new();
    public List<Item> OtherInventoryItems { get; set; } = new();
    public List<IDebuff> Debuffs { get; set; } = [];
    public string CharacterType { get; set; }
}

public static partial class  Mapper {
    public static PlayerViewModel ToViewModel(this Hero model)
    {
        return new PlayerViewModel()
        {
            Id = model.Id,
            Hp = model.Hp,
            Armor = model.Armor,
            Damage = model.Damage,
            DebuffResistance = model.DebuffResistance,
            Abilities = model.Abilities,
            Debuffs = model.Debuffs,
            CriticalDamage = model.CriticalDamage,
            CriticalChance = model.CriticalChance,
            CharacterType = model.CharacterType,
            DodgeChance = model.DodgeChance,
            Equipment = model.Equipment,
            InventoryEquipmentItems = model.Inventory.Where(i => i.ItemType == ItemType.Equipment).Cast<EquipmentBase>().ToList(),
            OtherInventoryItems = model.Inventory.Where(i => i.ItemType != ItemType.Equipment).ToList()
        };
    }
}