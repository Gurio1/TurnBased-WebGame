namespace Game.Contracts;

public sealed record MonsterAbilityCatalogItemViewModel
{
    public required string Id { get; init; }
    public required string TypeName { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
    public required int Cooldown { get; init; }
}
