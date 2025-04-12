using Game.Core.Models;
using Game.Features;
using Game.Features.Battle.Models;
using MediatR;

namespace Game.Core.AbilityEffects;

public interface IDebuff
{
    public int Duration { get; set; }
    public  string Name { get; set; }

    public void Execute(CombatEntity target, BattleContext context);
}