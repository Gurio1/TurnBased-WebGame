namespace Game.Core.Equipment.Generation;

public sealed class BlueprintAttributes
{
    public required List<BlueprintStatRange> Stats { get; init; }
    public required Dictionary<string, double> AttributeCountWeights { get; init; }
}
