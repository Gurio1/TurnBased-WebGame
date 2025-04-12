using Game.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Features.Attributes;

[BsonDiscriminator(Required = true)]
public abstract class EquipmentAttribute
{
    public abstract string Name { get; }
    public abstract float Value { get; set; }
    public abstract void ApplyStats(Player character);
    public abstract void RemoveStats(Player character);
}
public class AttackAttribute : EquipmentAttribute
{
    public override string Name { get; } = "Attack";
    public override float Value { get; set; }
    public override void ApplyStats(Player character)
    {
        character.Stats.Damage += Value;
    }

    public override void RemoveStats(Player character)
    {
        character.Stats.Damage -= Value;
    }
}

public class ArmorAttribute : EquipmentAttribute
{
    public override string Name { get; } = "Armor";
    public override float Value { get; set; }
    public override void ApplyStats(Player character)
    {
        character.Stats.Armor += Value;
    }
    
    public override void RemoveStats(Player character)
    {
        character.Stats.Armor -= Value;
    }
}

public class SpeedAttribute : EquipmentAttribute
{
    public override string Name { get; } = "Speed";
    public override float Value { get; set; }
    public override void ApplyStats(Player character)
    {
        throw new NotImplementedException();
    }
    public override void RemoveStats(Player character)
    {
        character.Stats.Damage -= Value;
    }
}

public class CriticalChanceAttribute : EquipmentAttribute
{
    public override string Name { get; } = "Critical chance";
    public override float Value { get; set; }
    public override void ApplyStats(Player character)
    {
        character.Stats.CriticalChance += Value;
    }
    public override void RemoveStats(Player character)
    {
        character.Stats.CriticalChance -= Value;
    }
}

public class CriticalDamageAttribute : EquipmentAttribute
{
    public override string Name { get; } = "Critical damage";
    public override float Value { get; set; }
    public override void ApplyStats(Player character)
    {
        character.Stats.CriticalDamage += Value;
    }
    
    public override void RemoveStats(Player character)
    {
        character.Stats.CriticalDamage -= Value;
    }
}