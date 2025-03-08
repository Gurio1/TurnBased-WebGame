using Game.Backpack;
using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;

namespace Game.Core.Models;

public class Hero : CharacterBase
{
    public Guid Id { get; set; }

    public override float Hp { get; set; } = 250;
    public override List<Ability> Abilities { get; set; } = [];
    public override List<IEquipment> Equipment { get; set; } = [];
    public List<IBackpackItem> BackpackItems { get; set; } = [];
    public override List<IDebuff> Debuffs { get; set; } = [];
    public override string CharacterType { get; } = "Hero";
}