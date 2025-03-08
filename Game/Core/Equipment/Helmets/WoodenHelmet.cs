using System.Globalization;
using Game.Core.Models;

namespace Game.Core.Equipment.Helmets;

internal class WoodenHelmet : Helmet
{
    public override string Name { get; set; } = "Wooden Helmet";
    
    public override int Value { get; } = 250;
    public override int CountOfOneStack { get; } = 1;

    public override float Armor { get; set; } = 5f;
    public override float Health { get; set; } = 25;
    
    public override float DropChance { get; set; } = 0.1f;
    
    public override Dictionary<string, string> Stats => new()
    {
        {nameof(Health), Health.ToString(CultureInfo.CurrentCulture)},
        {nameof(Armor), Armor.ToString(CultureInfo.CurrentCulture)},
    };

    
    public override void ApplyStats(CharacterBase characterBase)
    {
        characterBase.Hp += Health;
        characterBase.Armor += Armor;
    }

    public override void RemoveStats(CharacterBase characterBase)
    {
        characterBase.Hp -= Health;
        characterBase.Armor += Armor;
    }
}