namespace Game.Contracts;

public sealed record MonsterSummaryViewModel
{
    public required string Name { get; init; }
    public required double MaxHealth { get; init; }
    public required double Damage { get; init; }
    public required int AbilityCount { get; init; }
    public required int DropCount { get; init; }
}
