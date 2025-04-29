using System.Globalization;
using Game.Features.Attributes;
using Game.Utilities;

namespace Game.Core.Equipment.Generation.Steps;

public sealed class ApplyBlueprintStatsStep : IEquipmentPipelineStep
{
    public double Weight => 1.0;
    
    public bool CanApply(EquipmentGenerationContext context) => context.Blueprint is null;
    
    public EquipmentBase Apply(EquipmentGenerationContext context)
    {
        int attributeCount = GetWeightedRandom(context.Blueprint!.AttributeCountWeights);
        
        for (int i = 0; i < attributeCount; i++)
        {
            if (context.Blueprint.AttributeRanges.Count == 0) break;
            
            int index = RandomHelper.Instance.Next(context.Blueprint.AttributeRanges.Count);
            
            var range = context.Blueprint.AttributeRanges[index];
            context.Blueprint.AttributeRanges.RemoveAt(index);
            
            float randomValue = (float)Math.Round(RandomHelper.NextFloat(range.MinValue, range.MaxValue), 2);
            
            range.Stat.Value = range.Stat is CriticalChanceStat or CriticalDamageStat
                ? randomValue.RoundTo2()
                : randomValue.RoundTo1();
            
            context.Equipment.Attributes.Add(range.Stat);
        }
        
        return context.Equipment;
    }
    
    private static int GetWeightedRandom(Dictionary<string, double> attributeCountWeights)
    {
        double totalWeight = attributeCountWeights.Values.Sum();
        double randomValue = RandomHelper.Instance.NextDouble() * totalWeight;
        
        foreach (var entry in attributeCountWeights)
        {
            if (randomValue < entry.Value) return Convert.ToInt32(entry.Key, CultureInfo.InvariantCulture);
            randomValue -= entry.Value;
        }
        
        return Convert.ToInt32(attributeCountWeights.Keys.First(), CultureInfo.InvariantCulture);
    }
}
