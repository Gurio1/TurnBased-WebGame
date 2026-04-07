using Game.SharedKernel.Battle;

namespace Game.SharedKernel.Messaging;

public sealed class BattleStartSnapshotResponse
{
    public BattlePlayerSnapshot? Player { get; set; }
    public BattleMonsterSnapshot? Monster { get; set; }
    public string? ErrorMessage { get; set; }
}
