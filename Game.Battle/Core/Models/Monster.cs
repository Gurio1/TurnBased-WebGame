using Game.Battle.Core.Battle;

namespace Game.Battle.Core.Models;

public sealed class Monster : CombatEntity
{
    public double OverallDropChance { get; set; }
    public List<MonsterDropEntry> Drops { get; set; } = [];
}
