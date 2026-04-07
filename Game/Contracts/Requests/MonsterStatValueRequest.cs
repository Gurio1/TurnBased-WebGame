namespace Game.Contracts.Requests;

public sealed record MonsterStatValueRequest
{
    public required string Key { get; init; }
    public required double Value { get; init; }
}
