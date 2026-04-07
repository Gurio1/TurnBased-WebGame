namespace Game.Core.Equipment;

public class EquipmentBlueprint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string EquipmentId { get; set; }
    public required List<BlueprintStatRange> Stats { get; set; }
    public required Dictionary<string, double> AttributeCountWeights { get; set; }
}
