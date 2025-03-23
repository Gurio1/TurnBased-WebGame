using Game.Core.Models;
using MediatR;

namespace Game.Core.AbilityEffects;

public class Sleep : IDebuff
{
    public int Duration { get; set; }
    public string Name { get; set; } = "Sleep";

    public Sleep(int duration)
    {
        Duration = duration;
    }

    public void Execute(CharacterBase target, IMediator mediator)
    {
        Duration--;
    }
}