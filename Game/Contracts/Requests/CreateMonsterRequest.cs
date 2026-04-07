namespace Game.Contracts.Requests;

public sealed record CreateMonsterRequest
{
    public required string Name { get; init; }
    public double OverallDropChance { get; init; }
    public List<MonsterStatValueRequest> Stats { get; init; } = [];
    public List<MonsterDropEntryRequest> Drops { get; init; } = [];
    public required List<string> AbilityIds { get; init; }
}
