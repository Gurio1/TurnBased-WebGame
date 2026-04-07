namespace Game.Contracts;

public sealed record MonsterDetailViewModel
{
    public required string Name { get; init; }
    public required double OverallDropChance { get; init; }
    public List<MonsterStatValueViewModel> Stats { get; init; } = [];
    public List<string> AbilityIds { get; init; } = [];
    public List<MonsterDropEntryViewModel> Drops { get; init; } = [];
}
