namespace Game.SharedKernel.Battle;

public sealed class BattleMonsterDropSnapshot
{
    public string ItemTypeName { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double Weight { get; set; }
}
