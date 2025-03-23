using Game.Core.AbilityEffects;
using Game.Core.Equipment;

namespace Game.Core.Models;

public class Hero : CharacterBase
{
    public override float Hp { get; set; } = 250;
    public override float Armor { get; set; }
    public override float Damage { get; set; }
    public override float DebuffResistance { get; set; }
    public override float CriticalChance { get; set; }
    public override float CriticalDamage { get; set; }
    public override float DodgeChance { get; set; }
    public override List<string> AbilityIds { get; set; } = [];
    public override Dictionary<string, EquipmentBase?> Equipment { get; set; } = [];
    public List<Item> Inventory { get; set; } = [];
    public override List<IDebuff> Debuffs { get; set; } = [];
    public override string CharacterType { get; set; } = "Hero";

    public void Equip(EquipmentBase equipmentItem)
    {
        if (Equipment.TryGetValue(equipmentItem.Slot, out var equippedItem) && equippedItem is not null)
        {
            Inventory.Add(equippedItem);
            equippedItem.RemoveStats(this);
        }

        Equipment[equipmentItem.Slot] = equipmentItem;
        Inventory.Remove(equipmentItem);
        equipmentItem.ApplyStats(this);
    }
    
    public bool Unequip(string equipmentSlot)
    {
        if (!Equipment.TryGetValue(equipmentSlot, out EquipmentBase? equippedItem)) return false;
        
        
        if (equippedItem is null)
        {
            return false;
        }
            
        equippedItem.RemoveStats(this);
        Equipment.Remove(equipmentSlot);
        Inventory.Add(equippedItem);

        return true;

    }
}