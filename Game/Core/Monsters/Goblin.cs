using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Core.Equipment.Armors;
using Game.Core.Equipment.Boots;
using Game.Core.Equipment.Helmets;
using Game.Core.Equipment.Swords;
using Game.Core.Models;
using Game.Drop;
using Newtonsoft.Json;

namespace Game.Core.Monsters;

internal class Goblin : Monster
{
    public override string CharacterType { get; } = "Goblin";
    
    public override float Hp { get; set; } = 50;
    
    [JsonIgnore]
    public override int Level { get; set; }

    [JsonIgnore]
    public override List<IDropable> ListOfDrops { get; set; } =
        [new WoodenArmour(), new WoodenBoots(), new WoodenHelmet(), new WoodenSword()];

    public override List<Ability> Abilities { get; set; } = [];
    public override List<IEquipment> Equipment { get; set; } = [];
    public override List<IDebuff> Debuffs { get; set; } = [];
}