namespace Game.Contracts;

public sealed record MonsterStatCatalogItemViewModel
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string ValueType { get; init; }
    public required double DefaultValue { get; init; }
}
