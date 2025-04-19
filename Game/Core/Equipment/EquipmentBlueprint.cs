using MongoDB.Bson;

namespace Game.Core.Equipment;

public class EquipmentBlueprint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EquipmentId { get; set; } 
    public required List<AttributeRange> AttributeRanges { get; set; } 
    
    public required Dictionary<string, double> AttributeCountWeights { get; set; } 
}
