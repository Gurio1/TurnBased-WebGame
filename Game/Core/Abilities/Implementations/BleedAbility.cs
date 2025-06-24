using Game.Core.Battle;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Core.StatusEffects;
using Game.Utilities.Extensions;

namespace Game.Core.Abilities.Implementations;

public sealed class BleedAbility : Ability
{
    private const int Duration = 2;
    private const float AbilityDamageMultiplier = 0.7f;
    private const float BleedDamageMultiplier = 0.1f;
    public override string TypeName { get; init; } = nameof(BleedAbility);
    public override string Id { get; set; } = "1";
    public override string Name { get; set; } = "Bleed";
    public override string ImageUrl { get; set; } = "BleedSlash.png";
    public override int Cooldown { get; init; } = 4;
    public override int CurrentCooldown { get; protected set; }
    
    
    public override void Execute(CombatEntity owner, CombatEntity target, BattleContext context)
    {
        if (CurrentCooldown != 0) throw new InvalidOperationException("Called execution of ability which is in cd");
        
        CurrentCooldown = Cooldown;
        
        float damage = owner.CalculateDamage(GetAbilityDamage(owner.Stats), context);
        
        bool tookDamage = target.Defence(damage, context);
        
        if (tookDamage) ApplyBleedDebuff(target, GetBleedDamage(owner.Stats), context);
    }
    
    private static float GetAbilityDamage(Stats ownerStats) =>
        (ownerStats.Damage * AbilityDamageMultiplier).RoundTo1();
    
    private static float GetBleedDamage(Stats ownerStats) =>
        (ownerStats.Damage * BleedDamageMultiplier).RoundTo1();
    
    public override string GetAbilityDescription(GamePlayer gamePlayer) =>
        $"The player unleashes a bleeding slash, dealing {GetAbilityDamage(gamePlayer.Stats)} damage" +
        $" and inflicting a bleed effect that causes the target to take additional damage" +
        $" equal to 10% of the player's Damage stat: {GetBleedDamage(gamePlayer.Stats)} per turn for {Duration} turns.";
    
    private static void ApplyBleedDebuff(CombatEntity target, float damage, BattleContext context)
    {
        var debuff = new Bleed(Duration, damage);
        target.Debuffs.Add(debuff);
        
        context.PublishActionLog($"{target.CharacterType} has been debuffed with {debuff.Name}");
    }
}
