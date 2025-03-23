using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Core.Models;

namespace Game.Features.Battle.Contracts;

public class HeroBattleModel
{
    public string Id { get; init; }
    public string CharacterType { get; init; }

    public  float Hp { get; init; }
    public float Armor { get; init; }
    public float Damage { get; init; }
    public float DebuffResistance { get; init; }

    public float CriticalChance { get; init; } 
    public float CriticalDamage { get; init; } 
    public float DodgeChance { get; init; }
    
    public  List<Ability> Abilities{ get; init; }
    public Dictionary<string, EquipmentBase?> Equipment { get; set; }
    public List<Item> Inventory { get; init; }
    public  List<IDebuff> Debuffs { get; init; }
}

public static partial class Mapper{
    public static HeroBattleModel ToViewModel(this CharacterBase model,List<Ability> modelAbilities)
    {
        return new HeroBattleModel()
        {
            Id = model.Id,
            CharacterType = model.CharacterType,
            Hp = model.Hp,
            Armor = model.Armor,
            Damage = model.Damage,
            DebuffResistance = model.DebuffResistance,
            CriticalDamage = model.CriticalDamage,
            CriticalChance = model.CriticalChance,
            DodgeChance = model.DodgeChance,
            Abilities = modelAbilities,
            Debuffs = model.Debuffs
        };
    }
    public static Hero ToModel(this HeroBattleModel model)
    {
        return new Hero()
        {
            Id = model.Id,
            Hp = model.Hp,
            Armor = model.Armor,
            Damage = model.Damage,
            DebuffResistance = model.DebuffResistance,
            AbilityIds = model.Abilities.Select(a => a.Id).ToList(),
            Debuffs = model.Debuffs,
            CriticalDamage = model.CriticalDamage,
            CriticalChance = model.CriticalChance,
            CharacterType = model.CharacterType,
            DodgeChance = model.DodgeChance,
            Equipment =  model.Equipment,
            Inventory = model.Inventory
        };
    }
}