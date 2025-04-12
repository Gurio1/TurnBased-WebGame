using Game.Core.AbilityEffects;
using Game.Core.Models;
using Game.Features;
using Game.Features.Battle.Models;
using Game.Logger;
using MediatR;

namespace Game.Core.Abilities;

public sealed class BleedAbility : Ability
{
    public override string TypeName { get; init; } = nameof(BleedAbility);
    public override string Id { get; set; } = "1";
    public override string Name { get; set; } = "Bleed";
    public override string ImageUrl { get; set; } = "BleedSlash.png";
    public override int Cooldown { get; init; } = 4;
    public override int CurrentCooldown { get; set; }
    private const int Duration = 2;
    private const float AbilityDamageMultiplier = 0.7f;
    private const float BleedDamageMultiplier = 0.1f;


    public override void Execute(CombatEntity owner, CombatEntity target, BattleContext context)
    {
        if (CurrentCooldown != 0)
        {
            throw new Exception("Called execution of ability which is in cd");
        }
        
        CurrentCooldown = Cooldown;

        var damage = owner.CalculateDamage(GetAbilityDamage(owner.Stats),context);

        var tookDamage = target.Defence(damage,context);

        if (tookDamage)
        {
            ApplyBleedDebuff(target,GetBleedDamage(owner.Stats),context);
        }
    }

    private static float GetAbilityDamage(Stats ownerStats)
    {
        return (ownerStats.Damage *AbilityDamageMultiplier).RoundTo1();
    }

    private static float GetBleedDamage(Stats ownerStats)
    {
        return (ownerStats.Damage * BleedDamageMultiplier).RoundTo1();
    }

    public override string GetAbilityDescription(Player player)
    {
        return
            $"The player unleashes a bleeding slash, dealing {GetAbilityDamage(player.Stats)} damage" +
            $" and inflicting a bleed effect that causes the target to take additional damage" +
            $" equal to 10% of the player's Damage stat: {GetBleedDamage(player.Stats)} per turn for {Duration} turns.";
    }

    private void ApplyBleedDebuff(CombatEntity target,float damage, BattleContext context)
    {
        var debuff = new Bleed(Duration,damage);
        target.Debuffs.Add(debuff);
        
        context.PublishActionLog($"{target.CharacterType} has been debuffed with {debuff.Name}");
    }
}