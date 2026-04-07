using System.Globalization;
using Game.SharedKernel.Utilities;
using Game.SharedKernel.Utilities.Extensions;
using Game.Utilities.Extensions;

namespace Game.Core.Equipment.Generation.Steps;

public sealed class ApplyBlueprintStatsStep : IEquipmentPipelineStep
{
    public double Weight => 1.0;
    
    public bool CanApply(EquipmentGenerationContext context) => context.Blueprint is not null;
    
    public EquipmentBase Apply(EquipmentGenerationContext context)
    {
        int attributeCount = GetWeightedRandom(context.Blueprint!.AttributeCountWeights);
        
        for (int i = 0; i < attributeCount; i++)
        {
            if (context.Blueprint.Stats.Count == 0) break;
            
            int index = RandomHelper.Instance.Next(context.Blueprint.Stats.Count);
            
            var range = context.Blueprint.Stats[index];
            context.Blueprint.Stats.RemoveAt(index);
            
            float randomValue = (float)Math.Round(RandomHelper.NextFloat(range.MinValue, range.MaxValue), 2);

            var stat = EquipmentStatRegistry.Create(range.StatKey);
            stat.Value = stat switch
            {
                CriticalChanceStat => CriticalStatPercentages.NormalizeCriticalChance(randomValue),
                CriticalDamageStat => CriticalStatPercentages.NormalizeCriticalDamage(randomValue),
                _ => randomValue.RoundTo1()
            };

            context.Equipment.Attributes.Add(stat);
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
