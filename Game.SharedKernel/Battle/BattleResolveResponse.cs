namespace Game.SharedKernel.Battle;

public sealed class BattleResolveResponse
{
    public BattleRewardDto? Reward { get; set; }
    public string? ErrorMessage { get; set; }
}
