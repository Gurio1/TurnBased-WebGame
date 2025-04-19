using Game.Features.Battle.Models;

namespace Game.Core.AbilityEffects;

public sealed class Bleed : IDebuff
{
    public int Duration { get; set; }
    public string Name { get; set; } = "Bleed";
    public float Damage { get; }
    
    public Bleed(int duration,float damage)
    {
        Damage = damage;
        Duration = duration;
    }
    public void Execute(CombatEntity target, BattleContext context)
    {
        context.PublishActionLog($"{target.CharacterType} took {Damage} damage from {Name}");
        target.CalculateDamage(Damage,context);

        Duration--;
    }
}
