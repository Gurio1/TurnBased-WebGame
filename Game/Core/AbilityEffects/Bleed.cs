using Game.Features.Battle.Models;

namespace Game.Core.AbilityEffects;

public sealed class Bleed : IDebuff
{
    public Bleed(int duration, float damage)
    {
        Damage = damage;
        Duration = duration;
    }
    
    public float Damage { get; }
    public int Duration { get; set; }
    public string Name { get; set; } = "Bleed";
    
    public void Execute(CombatEntity target, BattleContext context)
    {
        context.PublishActionLog($"{target.CharacterType} took {Damage} damage from {Name}");
        target.CalculateDamage(Damage, context);
        
        Duration--;
    }
}
