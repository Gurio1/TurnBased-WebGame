using System.Globalization;
using Game.Core.Common;
using Game.Core.Equipment;
using Game.Features.Attributes;
using Game.Utilities;

namespace Game.Features.Equipment;

public class EquipmentGenerator : IEquipmentGenerator
{
    private readonly IDispatcher dispatcher;
    
    public EquipmentGenerator(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public async Task<Result<EquipmentBase>> GenerateEquipment(string equipmentType)
    {
        var equipmentResult = EquipmentFactory.CreateDrop(equipmentType);
        
        if (equipmentResult.IsFailure) return equipmentResult;
        
        var blueprintResult = await dispatcher.Dispatch(new GetBlueprintQuery(equipmentResult.Value.EquipmentId));
        
        if (blueprintResult.IsFailure) return blueprintResult.AsError<EquipmentBase>();
        
        var blueprintAttributes = blueprintResult.Value;
        
        int attributeCount = GetWeightedRandom(blueprintAttributes.AttributeCountWeights);
        
        for (int i = 0; i < attributeCount; i++)
        {
            if (blueprintAttributes.AttributeRanges.Count == 0) break;
            
            int index = RandomHelper.Instance.Next(blueprintAttributes.AttributeRanges.Count);
            
            var range = blueprintAttributes.AttributeRanges[index];
            blueprintAttributes.AttributeRanges.RemoveAt(index);
            
            float randomValue = (float)Math.Round(RandomHelper.NextFloat(range.MinValue, range.MaxValue), 2);
            
            range.Stat.Value = range.Stat is CriticalChanceStat or CriticalDamageStat
                ? randomValue.RoundTo2()
                : randomValue.RoundTo1();
            
            equipmentResult.Value.Attributes.Add(range.Stat);
        }
        
        return equipmentResult;
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
