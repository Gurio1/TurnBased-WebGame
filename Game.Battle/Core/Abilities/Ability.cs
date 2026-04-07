using Game.Battle.Core.Battle;

namespace Game.Battle.Core.Abilities;

public abstract class Ability
{
    public abstract string TypeName { get; init; }
    public abstract string Id { get; set; }
    public abstract string Name { get; set; }
    public abstract string ImageUrl { get; set; }
    public abstract int Cooldown { get; init; }
    public abstract int CurrentCooldown { get; protected set; }
    
    public abstract void Execute(CombatEntity owner, CombatEntity target, BattleContext context);
    
    public void DecreaseCurrentCooldown()
    {
        if (CurrentCooldown != 0) CurrentCooldown--;
    }
}
