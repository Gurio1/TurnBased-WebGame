namespace Game.SharedKernel.Battle;

public sealed class BattleLootItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int MaxInventorySlotQuantity { get; set; }
    public int Quantity { get; set; }
}
