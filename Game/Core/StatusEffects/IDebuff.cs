using Game.Core.Battle;

namespace Game.Core.StatusEffects;

public interface IDebuff
{
    public int Duration { get; set; }
    public string Name { get; set; }
    
    public void Execute(CombatEntity target, BattleContext context);
}
