namespace Game.SharedKernel.Contracts.Requests;

public sealed record BattleStartSnapshotRequest
{
    public string PlayerId { get; init; } = string.Empty;
    public string MonsterName { get; init; } = string.Empty;
}
