using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Features.Battle.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Models;

public class Monster : CombatEntity, IHasAbilityIds
{
    public Dictionary<string, float> DropsTable { get; init; }
    
    [JsonIgnore] public List<string> AbilityIds { get; set; } = [];
    
    [BsonIgnore] public Ability?[] Abilities { get; set; } = [];
    
    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Equipment ??= new Dictionary<string, EquipmentBase?>();
        Debuffs ??= new List<IDebuff>();
    }
}
