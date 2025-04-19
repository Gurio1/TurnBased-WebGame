using MongoDB.Bson;

namespace Game.Core.Equipment;

public class EquipmentBlueprint
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public string EquipmentId { get; set; } // e.g., "wooden-sword"
    public required List<AttributeRange> AttributeRanges { get; set; } 
    
    public required Dictionary<string, double> AttributeCountWeights { get; set; } 
}
