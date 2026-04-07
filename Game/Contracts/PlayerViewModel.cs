using Game.SharedKernel.Models;

namespace Game.Contracts;

public class PlayerViewModel
{
    public string Id { get; set; }
    public Stats Stats { get; set; }
    public Dictionary<string, EquipmentViewModel?> Equipment { get; set; } = [];
    public List<InventorySlotViewModel> Inventory { get; set; } = new();
    public string CharacterType { get; set; }
}
