using Game.Core.AbilityEffects;
using Game.Core.Models;
using Game.Features.Battle.Models;

namespace Game.Core.Abilities;

public class SleepAbility : Ability
{
    public override string TypeName { get; init; } = nameof(SleepAbility);
    
    public override string Id { get; set; } = "2";
    public override string Name { get; set; } = "Sleep";
    public override string ImageUrl { get; set; }
    public override int Cooldown { get; init; } = 4;
    public override int CurrentCooldown { get; set; }
    private int Duration { get; } = 2;
    
    public override void Execute(CombatEntity owner, CombatEntity target, BattleContext context)
    {
        if (CurrentCooldown != 0)
        {
            Console.WriteLine("Can not use this ability");
            return;
        }
        
        float damage = owner.CalculateDamage(owner.Stats.Damage * 0.7f, context);
        
        bool tookDamage = target.Defence(damage, context);
        
        if (tookDamage) SetDebuff(target, context);
        
        CurrentCooldown = Cooldown;
    }
    
    public override string GetAbilityDescription(Player player) =>
        throw new NotImplementedException();
    
    private void SetDebuff(CombatEntity target, BattleContext context)
    {
        var debuff = new Sleep(Duration);
        target.Debuffs.Add(debuff);
        
        context.PublishActionLog($"{target.CharacterType} has been debuffed with {debuff.Name}");
    }
}
