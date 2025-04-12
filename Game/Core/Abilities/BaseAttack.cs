using Game.Core.Models;
using Game.Features;
using Game.Features.Battle.Models;
using MediatR;

namespace Game.Core.Abilities;

public class BaseAttack : Ability
{
    public override string Id { get; set; } = "0";
    public override string TypeName { get; init; } = nameof(BaseAttack);
    public override string Name { get; set; } = "BaseAttack";
    public override string ImageUrl { get; set; } = "BaseAttack.png";
    public override int Cooldown { get; init; } = 0;
    public override int CurrentCooldown { get; set; } = 0;

    public override void Execute(CombatEntity owner, CombatEntity target, BattleContext context)
    {
        var damage = owner.CalculateDamage(owner.Stats.Damage,context);
        target.Defence(damage,context);
    }

    public override string GetAbilityDescription(Player player)
    {
        return $"Performs a basic attack that deals damage equal to the player's Damage stat: {player.Stats.Damage}.";
    }
}