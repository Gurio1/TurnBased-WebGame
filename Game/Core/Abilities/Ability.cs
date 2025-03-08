using Game.Core.Models;
using MediatR;
using Newtonsoft.Json;

namespace Game.Core.Abilities;

public abstract class Ability() : ICreatableClass
{
    public abstract string TypeName { get; init; }
    public abstract int Id { get; set; } 
    public abstract string Name { get; set; }
    public abstract int Cooldown  { get; init; }
    public abstract int CurrentCooldown { get; set; }
    
    public abstract float Execute(CharacterBase owner, CharacterBase target, IMediator mediator);

    public void DecreaseCurrentCooldown()
    {
        if (CurrentCooldown != 0)
        {
            CurrentCooldown--;
        }
    }
}