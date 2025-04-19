using Game.Features.Battle.Models;

namespace Game.Core.AbilityEffects;

public class Sleep : IDebuff
{
    public int Duration { get; set; }
    public string Name { get; set; } = "Sleep";

    public Sleep(int duration) => Duration = duration;
    
    public void Execute(CombatEntity target, BattleContext context) => Duration--;
}
