using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Game.Core.Equipment;

public class EquipmentTemplate
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public string EquipmentId { get; set; } // e.g., "wooden-sword"
    public required List<EquipmentAttributeRange> AttributeRanges { get; set; } 
    
    public required Dictionary<string, double> AttributeCountWeights { get; set; } 
}