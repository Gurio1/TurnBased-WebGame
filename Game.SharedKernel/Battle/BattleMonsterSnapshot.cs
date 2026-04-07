using Game.SharedKernel.Models;

namespace Game.SharedKernel.Battle;

public sealed class BattleMonsterSnapshot
{
    public string Name { get; set; } = string.Empty;
    public Stats Stats { get; set; } = new();
    public List<string> AbilityIds { get; set; } = [];
    public double OverallDropChance { get; set; }
    public List<BattleMonsterDropSnapshot> Drops { get; set; } = [];
}
