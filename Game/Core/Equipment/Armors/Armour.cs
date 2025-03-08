using Game.Core.Models;

namespace Game.Core.Equipment.Armors;

public abstract class Armour : IEquipment
{
    public string EquipmentType { get; set; } = "Armour";
    public abstract string Name { get; set; }
    
    public abstract int Value { get; }
    public abstract int CountOfOneStack { get; }

    public abstract float Armor { get; set; }
    public abstract float Health { get; set; }
    public abstract float DebuffResistance { get; set; }
    
    public abstract float DropChance { get; set; }
    
    public abstract Dictionary<string,string>  Stats { get; }
    public abstract void ApplyStats(CharacterBase characterBase);
    public abstract void RemoveStats(CharacterBase characterBase);
    
    public void HandleDrop(Hero hero)
    {
        hero.BackpackItems.Add(this);
    }
}