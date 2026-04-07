using Game.SharedKernel.Models;

namespace Game.SharedKernel.Battle;

public sealed class BattlePlayerSnapshot
{
    public string Id { get; set; } = string.Empty;
    public string? BattleId { get; set; }
    public Stats Stats { get; set; } = new();
    public List<string> AbilityIds { get; set; } = [];
    public string CharacterType { get; set; } = string.Empty;
}
