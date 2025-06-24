using Game.Core.Marketplace;
using Game.Core.Models;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Equipment;

[BsonDiscriminator(Required = true)]
public abstract class EquipmentBase : Item
{
    public abstract string EquipmentId { get; set; }
    public override Currency SellPrice { get; set; } = new Gold {Amount = 200};
    public override ItemType ItemType { get; set; } = ItemType.Equipment;
    public abstract string Slot { get; set; }
    public override ItemInteractions Interactions { get; } = ItemInteractions.Equip | ItemInteractions.Sell;
    public List<EquipmentStat> Attributes { get; set; } = new();
    
    public override int MaxInventorySlotQuantity { get; set; } = 1;
    
    public void ApplyStats(GamePlayer characterBase) => Attributes.ForEach(a => a.ApplyStats(characterBase));
    
    public void RemoveStats(GamePlayer characterBase) => Attributes.ForEach(a => a.RemoveStats(characterBase));
}
