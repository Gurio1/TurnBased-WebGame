using Game.Core.Battle;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Core.StatusEffects;

namespace Game.Core.Abilities.Implementations;

public class SleepAbility : Ability
{
    public override string TypeName { get; init; } = nameof(SleepAbility);
    
    public override string Id { get; set; } = "2";
    public override string Name { get; set; } = "Sleep";
    public override string ImageUrl { get; set; }
    public override int Cooldown { get; init; } = 4;
    public override int CurrentCooldown { get; protected set; }
    private int Duration { get; } = 2;
    
    public override void Execute(CombatEntity owner, CombatEntity target, BattleContext context)
    {
        if (CurrentCooldown != 0)
        {
            context.PublishActionLog($"Can not use {Name} ability");
            return;
        }
        
        float damage = owner.CalculateDamage(owner.Stats.Damage * 0.7f, context);
        
        bool tookDamage = target.Defence(damage, context);
        
        if (tookDamage) SetDebuff(target, context);
        
        CurrentCooldown = Cooldown;
    }
    
    public override string GetAbilityDescription(GamePlayer gamePlayer) =>
        throw new NotImplementedException();
    
    private void SetDebuff(CombatEntity target, BattleContext context)
    {
        var debuff = new Sleep(Duration);
        target.Debuffs.Add(debuff);
        
        context.PublishActionLog($"{target.CharacterType} has been debuffed with {debuff.Name}");
    }
}
