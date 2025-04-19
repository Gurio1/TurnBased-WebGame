using Game.Core.Equipment;

namespace Game.Features.Equipment;

public sealed class BlueprintAttributes
{
    public List<AttributeRange> AttributeRanges { get; init; }
    public Dictionary<string, double> AttributeCountWeights { get; init; }
}
