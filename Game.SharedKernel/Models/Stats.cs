using Game.SharedKernel.Serialization.Bson;
using Game.SharedKernel.Serialization.NewtonsoftJson;
using Game.SharedKernel.Serialization.SystemTextJson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.SharedKernel.Models;

public class Stats
{
    public float Armor { get; set; }

    [BsonSerializer(typeof(CriticalChancePercentageBsonSerializer))]
    [System.Text.Json.Serialization.JsonConverter(typeof(CriticalChancePercentageJsonConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(CriticalChancePercentageNewtonsoftConverter))]
    public int CriticalChance { get; set; }

    [BsonSerializer(typeof(CriticalDamagePercentageBsonSerializer))]
    [System.Text.Json.Serialization.JsonConverter(typeof(CriticalDamagePercentageJsonConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(CriticalDamagePercentageNewtonsoftConverter))]
    public int CriticalDamage { get; set; }

    public float CurrentHealth { get; set;}

    public float Damage { get; set;}

    public float DebuffResistance { get; set;}

    public float DodgeChance { get; set;}
    public float MaxHealth { get; set;}
    public float Speed { get; set; }
}
