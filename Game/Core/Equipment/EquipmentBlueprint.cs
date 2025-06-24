namespace Game.Core.Equipment;

public class EquipmentBlueprint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string EquipmentId { get; init; }
    public required List<AttributeRange> AttributeRanges { get; init; }
    
    public required Dictionary<string, double> AttributeCountWeights { get; init; }
}
