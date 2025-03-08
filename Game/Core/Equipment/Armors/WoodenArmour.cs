using System.Globalization;
using Game.Core.Models;

namespace Game.Core.Equipment.Armors;

public class WoodenArmour : Armour
{
    
    public override string Name { get; set; } = "Wooden Armour";
    public override int Value { get; } = 150;
    public override int CountOfOneStack { get; } = 1;

    public override float Armor { get; set; } = 5f;
    public override float Health { get; set; } = 25f;
    public override float DebuffResistance { get; set; } = 10f;
    
    public override float DropChance { get; set; } = 0.1f;
    
    public override Dictionary<string, string> Stats => new()
    {
        { nameof(Name), Name },
        { nameof(Armor), Armor.ToString(CultureInfo.CurrentCulture) },
        { nameof(Health), Health.ToString(CultureInfo.CurrentCulture) },
        { "Debuff Resistance", DebuffResistance.ToString(CultureInfo.CurrentCulture) },
        { "Gold", Value.ToString() }
    };

    
    public override void ApplyStats(CharacterBase characterBase)
    {
        characterBase.Armor += Armor;
        characterBase.Hp += Health;
    }

    public override void RemoveStats(CharacterBase characterBase)
    {
        characterBase.Armor -= Armor;
        characterBase.Hp -= Health;
    }
}