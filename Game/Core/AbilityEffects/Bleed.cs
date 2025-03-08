using Game.Core.Models;
using Game.Logger;
using MediatR;

namespace Game.Core.AbilityEffects;

internal class Bleed : IDebuff
{
    public int Duration { get; set; }
    public string Name { get; set; } = "Bleed";
    private float _damage = 5f;

    public Bleed(int duration)
    {
        Duration = duration;
    }
    public void Execute(CharacterBase target, IMediator mediator)
    {
        mediator.Publish(new ActionLogNotification($"{target.CharacterType} took {_damage} damage from {Name}"));
        target.CalculateDamage(_damage,mediator);

        Duration--;
    }
}