using Game.SharedKernel.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Models;

[BsonIgnoreExtraElements]
public class Monster
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public required string Name { get; set; }
    public Stats Stats { get; set; } = new();
    public double OverallDropChance { get; set; } = 1;
    public List<MonsterDropEntry> Drops { get; set; } = [];
    public List<string> AbilityIds { get; set; } = [];
}
