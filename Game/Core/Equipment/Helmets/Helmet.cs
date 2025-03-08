using Game.Core.Models;

namespace Game.Core.Equipment.Helmets;

public abstract class Helmet : IEquipment
{
    public string EquipmentType { get; set; } = "Helmet";
    
    public abstract string Name { get; set; }
    public abstract float Armor { get; set; }
    public abstract float Health { get; set; }
    
    public abstract int CountOfOneStack { get; }
    public abstract int Value { get; }
    
    public abstract float DropChance { get; set; }
    
    public abstract Dictionary<string,string>  Stats { get; }
    public abstract void ApplyStats(CharacterBase characterBase);
    public abstract void RemoveStats(CharacterBase characterBase);
    
    public void HandleDrop(Hero hero)
    {
        hero.BackpackItems.Add(this);
    }
    
}