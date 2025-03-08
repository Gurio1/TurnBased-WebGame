using System.Globalization;
using Game.Core.Models;

namespace Game.Core.Equipment.Boots;

internal class WoodenBoots : Boots
{
    public override string Name { get; set; } = "Wooden Boots";
    
    public override int Value { get; } = 250;
    public override int CountOfOneStack { get; } = 1;
    
    public override float Health { get; set; } = 15f;
    public override float DebuffResistance { get; set; } = 10f;
    
    public override float DropChance { get; set; } = 0.1f;
    
    public override Dictionary<string, string> Stats => new()
    {
        { nameof(Name), Name },
        { nameof(Health), Health.ToString(CultureInfo.CurrentCulture) },
        { "Debuff Resistance", DebuffResistance.ToString(CultureInfo.CurrentCulture) },
        { "Gold", Value.ToString() }
    };

    
    public override void ApplyStats(CharacterBase characterBase)
    {
        characterBase.Hp += Health;
        characterBase.DebuffResistance += DebuffResistance;
    }

    public override void RemoveStats(CharacterBase characterBase)
    {
        characterBase.Hp -= Health;
        characterBase.DebuffResistance -= DebuffResistance;
    }
    
}