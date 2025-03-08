using System.Globalization;
using Game.Core.Models;

namespace Game.Core.Equipment.Swords;

internal class WoodenSword : Sword
{
    public override string Name { get; set; } = "Wooden sword";

    public override float Damage { get; set; } = 10f;

    public override int CountOfOneStack { get; } = 1;
    public override int Value { get; } = 250;
    
    public override float DropChance { get; set; } = 0.1f;
    
    public override Dictionary<string, string> Stats => new()
    {
        { nameof(Name), Name },
        { nameof(Damage), Damage.ToString(CultureInfo.CurrentCulture) },
        { "Gold", Value.ToString() }
    };

    public override void ApplyStats(CharacterBase characterBase)
    {
        characterBase.Damage += Damage;
    }

    public override void RemoveStats(CharacterBase characterBase)
    {
        characterBase.Damage -= Damage;
    }

   

}