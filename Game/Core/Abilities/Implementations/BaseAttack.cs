using Game.Core.Battle;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;

namespace Game.Core.Abilities.Implementations;

public class BaseAttack : Ability
{
    public override string Id { get; set; } = "0";
    public override string TypeName { get; init; } = nameof(BaseAttack);
    public override string Name { get; set; } = "BaseAttack";
    public override string ImageUrl { get; set; } = "BaseAttack.png";
    public override int Cooldown { get; init; }
    public override int CurrentCooldown { get; protected set; }
    
    public override void Execute(CombatEntity owner, CombatEntity target, BattleContext context)
    {
        float damage = owner.CalculateDamage(owner.Stats.Damage, context);
        target.Defence(damage, context);
    }
    
    public override string GetAbilityDescription(GamePlayer gamePlayer) =>
        $"Performs a basic attack that deals damage equal to the player's Damage stat: {gamePlayer.Stats.Damage}.";
}
