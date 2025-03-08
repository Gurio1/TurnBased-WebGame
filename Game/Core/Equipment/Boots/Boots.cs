using Game.Core.Models;

namespace Game.Core.Equipment.Boots;

public abstract class Boots : IEquipment
{
    public string EquipmentType { get; set; } = "Boots";
    public abstract string Name { get; set; }
    public abstract int CountOfOneStack { get; }
    public abstract int Value { get; }
    

    public abstract float Health { get; set; }
    public abstract float DebuffResistance { get; set; }
    
    public abstract float DropChance { get; set; }

    public abstract Dictionary<string,string>  Stats { get; }
    
    public void HandleDrop(Hero hero)
    {
        hero.BackpackItems.Add(this);
    }

    public abstract void ApplyStats(CharacterBase characterBase);

    public abstract void RemoveStats(CharacterBase characterBase);
    
}