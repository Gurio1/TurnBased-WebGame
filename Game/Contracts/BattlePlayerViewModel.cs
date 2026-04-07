using Game.SharedKernel.Models;

namespace Game.Contracts;

public sealed class BattlePlayerViewModel
{
    public string Id { get; set; } = string.Empty;
    public Stats Stats { get; set; } = new();
    public List<string> AbilityIds { get; set; } = [];
    public string CharacterType { get; set; } = "Player";
}
