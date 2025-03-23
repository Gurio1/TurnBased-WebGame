using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Core.Equipment.Boots;
using Game.Core.Models;
using Newtonsoft.Json;

namespace Game.Core.Monsters;


//TODO : The same idea what meant about ListOfDrops
public class Goblin : Monster
{
    public override string CharacterType { get; set; } = "Goblin";
    
    public override float Hp { get; set; } = 150;
    public override float Armor { get; set; }
    public override float Damage { get; set; }
    public override float DebuffResistance { get; set; }
    public override float CriticalChance { get; set; }
    public override float CriticalDamage { get; set; }
    public override float DodgeChance { get; set; }

    public override Dictionary<string, float> DropsTable { get; init; } = new()
    {
        { "wooden-sword", 1f },
    };

    public override List<string> AbilityIds { get; set; }
    public override Dictionary<string, EquipmentBase> Equipment { get; set; }
    public override List<IDebuff> Debuffs { get; set; } = [];
}