using Game.Core.Models;
using MediatR;

namespace Game.Core.AbilityEffects;

public interface IDebuff
{
    public int Duration { get; set; }
    public  string Name { get; set; }

    public void Execute(CharacterBase target, IMediator mediator);
}