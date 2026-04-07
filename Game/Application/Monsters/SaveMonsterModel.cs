namespace Game.Application.Monsters;

public sealed class SaveMonsterModel
{
    public required string Name { get; init; }
    public double OverallDropChance { get; init; }
    public List<MonsterStatValueModel> Stats { get; init; } = [];
    public List<MonsterDropModel> Drops { get; init; } = [];
    public List<string> AbilityIds { get; init; } = [];
}

public sealed record MonsterStatValueModel(string Key, double Value);

public sealed record MonsterDropModel(string ItemTypeName, string ItemId, int Quantity, double Weight);
