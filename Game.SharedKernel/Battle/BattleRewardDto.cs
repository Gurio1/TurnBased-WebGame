namespace Game.SharedKernel.Battle;

public sealed class BattleRewardDto
{
    public int Gold { get; set; }
    public float Experience { get; set; }
    public List<BattleLootItemDto>? Drop { get; set; }
    public List<BattleEquipmentDto>? EquipmentDrop { get; set; }
}
