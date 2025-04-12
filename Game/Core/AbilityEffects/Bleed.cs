using Game.Core.Models;
using Game.Features;
using Game.Features.Battle.Models;
using Game.Logger;
using MediatR;

namespace Game.Core.AbilityEffects;

public sealed class Bleed : IDebuff
{
    public int Duration { get; set; }
    public string Name { get; set; } = "Bleed";
    public float Damage => _damage;
    private readonly float _damage;

    public Bleed(int duration,float damage)
    {
        _damage = damage;
        Duration = duration;
    }
    public void Execute(CombatEntity target, BattleContext context)
    {
        context.PublishActionLog($"{target.CharacterType} took {_damage} damage from {Name}");
        target.CalculateDamage(_damage,context);

        Duration--;
    }
}