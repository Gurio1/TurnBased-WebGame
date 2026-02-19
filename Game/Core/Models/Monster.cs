using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Game.Core.Abilities;
using Game.Core.Battle;
using Game.Core.Equipment;
using Game.Core.StatusEffects;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Models;

public class Monster : CombatEntity, IHasAbilityIds
{
    public Dictionary<string, float> DropsTable { get; init; }
    
    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Equipment ??= new Dictionary<string, EquipmentBase?>();
        Debuffs ??= [];
    }
}
