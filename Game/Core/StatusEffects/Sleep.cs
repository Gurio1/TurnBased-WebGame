using Game.Core.Battle;

namespace Game.Core.StatusEffects;

public class Sleep : IDebuff
{
    public Sleep(int duration) => Duration = duration;
    public int Duration { get; set; }
    public string Name { get; set; } = "Sleep";
    
    public void Execute(CombatEntity target, BattleContext context) => Duration--;
}
