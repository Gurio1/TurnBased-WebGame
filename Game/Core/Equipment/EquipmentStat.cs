using Game.Core.PlayerProfile;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Equipment;

[BsonDiscriminator(Required = true)]
public abstract class EquipmentStat
{
    public abstract string Name { get; }
    public abstract float Value { get; set; }
    public abstract void ApplyStats(Player character);
    public abstract void RemoveStats(Player character);
}

public class AttackStat : EquipmentStat
{
    public override string Name { get; } = "Attack";
    public override float Value { get; set; }
    public override void ApplyStats(Player character) => character.Stats.Damage += Value;
    
    public override void RemoveStats(Player character) => character.Stats.Damage -= Value;
}

public class ArmorStat : EquipmentStat
{
    public override string Name { get; } = "Armor";
    public override float Value { get; set; }
    public override void ApplyStats(Player character) => character.Stats.Armor += Value;
    
    public override void RemoveStats(Player character) => character.Stats.Armor -= Value;
}

public class SpeedStat : EquipmentStat
{
    public override string Name { get; } = "Speed";
    public override float Value { get; set; }
    
    public override void ApplyStats(Player character) => throw new NotImplementedException();
    
    public override void RemoveStats(Player character) => character.Stats.Damage -= Value;
}

public class CriticalChanceStat : EquipmentStat
{
    public override string Name { get; } = "Critical chance";
    public override float Value { get; set; }
    public override void ApplyStats(Player character) => character.Stats.CriticalChance += Value;
    
    public override void RemoveStats(Player character) => character.Stats.CriticalChance -= Value;
}

public class CriticalDamageStat : EquipmentStat
{
    public override string Name { get; } = "Critical damage";
    public override float Value { get; set; }
    public override void ApplyStats(Player character) => character.Stats.CriticalDamage += Value;
    
    public override void RemoveStats(Player character) => character.Stats.CriticalDamage -= Value;
}
