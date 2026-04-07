namespace Game.Core.Equipment;

public sealed class BlueprintStatRange
{
    public required string StatKey { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
}
