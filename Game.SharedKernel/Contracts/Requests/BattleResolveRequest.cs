using Game.SharedKernel.Models;

namespace Game.SharedKernel.Contracts.Requests;

public sealed record BattleResolveRequest
{
    public string PlayerId { get; init; } = string.Empty;
    public string BattleId { get; init; } = string.Empty;
    public string MonsterName { get; init; } = string.Empty;
    public bool Won { get; init; }
    public Stats PlayerStats { get; init; } = new();
    public Dictionary<string, int> UsedItems { get; init; } = new();
}
