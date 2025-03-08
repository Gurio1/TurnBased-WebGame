using Game.Core.Models;

namespace Game.Core.Equipment.Swords;

public abstract class Sword : IEquipment
{
    public string EquipmentType { get; set; } = "Sword";

    public abstract string Name { get; set; }
    public abstract float Damage { get; set; }
    
    public abstract int CountOfOneStack { get; }
    public abstract int Value { get; }
    
    public abstract float DropChance { get; set; }

    public abstract void ApplyStats(CharacterBase characterBase);
    public abstract void RemoveStats(CharacterBase characterBase);
    public abstract Dictionary<string,string>  Stats { get; }
    
    public void HandleDrop(Hero hero)
    {
        hero.BackpackItems.Add(this);
    }

}