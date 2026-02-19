using Game.Core.Marketplace;
using Game.Core.Models;
using Game.Core.PlayerProfile.Aggregates;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Equipment;

public abstract class EquipmentBase : Item, ISellable, IEquippable
{
    public abstract string EquipmentId { get; set; }
    public Currency SellPrice { get; set; } = new Gold { Quantity = 200 };
    public abstract string Slot { get; set; }
    public List<EquipmentStat> Attributes { get; set; } = [];
    public override int MaxInventorySlotQuantity { get; protected set; } = 1;
    public void ApplyStats(GamePlayer characterBase) => Attributes.ForEach(a => a.ApplyStats(characterBase));
    
    public void RemoveStats(GamePlayer characterBase) => Attributes.ForEach(a => a.RemoveStats(characterBase));
}
