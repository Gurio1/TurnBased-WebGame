namespace Game.Core.Equipment.Generation;

public sealed class BlueprintAttributes
{
    public List<AttributeRange> AttributeRanges { get; init; }
    public Dictionary<string, double> AttributeCountWeights { get; init; }
}
