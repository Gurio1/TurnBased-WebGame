using Game.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Features.Attributes;

[BsonDiscriminator(Required = true)]
public abstract class EquipmentAttribute
{
    public abstract float Value { get; set; }
    public abstract void ApplyStats(CharacterBase character);
    public abstract void RemoveStats(CharacterBase character);
}
public class AttackAttribute : EquipmentAttribute
{
    public override float Value { get; set; }
    public override void ApplyStats(CharacterBase character)
    {
        character.Damage += Value;
    }

    public override void RemoveStats(CharacterBase character)
    {
        character.Damage -= Value;
    }
}

public class DefenseAttribute : EquipmentAttribute
{
    public override float Value { get; set; }
    public override void ApplyStats(CharacterBase character)
    {
        character.Armor += Value;
    }
    
    public override void RemoveStats(CharacterBase character)
    {
        character.Armor -= Value;
    }
}

public class SpeedAttribute : EquipmentAttribute
{
    public override float Value { get; set; }
    public override void ApplyStats(CharacterBase character)
    {
        throw new NotImplementedException();
    }
    public override void RemoveStats(CharacterBase character)
    {
        character.Damage -= Value;
    }
}

public class CriticalChanceAttribute : EquipmentAttribute
{
    public override float Value { get; set; }
    public override void ApplyStats(CharacterBase character)
    {
        character.CriticalChance += Value;
    }
    public override void RemoveStats(CharacterBase character)
    {
        character.CriticalChance -= Value;
    }
}

public class CriticalDamageAttribute : EquipmentAttribute
{
    public override float Value { get; set; }
    public override void ApplyStats(CharacterBase character)
    {
        character.CriticalDamage += Value;
    }
    
    public override void RemoveStats(CharacterBase character)
    {
        character.CriticalDamage -= Value;
    }
}