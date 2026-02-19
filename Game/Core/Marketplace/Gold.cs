namespace Game.Core.Marketplace;

public sealed class Gold : Currency
{
    public override string Id { get; set; } = "Some-gold-id";
    public override string Name { get; set; } = nameof(Gold);
    public override string ImageUrl { get; init; } = "gold";
    public override int MaxInventorySlotQuantity { get; protected set; } = 50_000;
}
