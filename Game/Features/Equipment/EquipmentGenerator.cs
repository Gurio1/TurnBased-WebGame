using System.Globalization;
using Game.Core;
using Game.Core.Common;
using Game.Core.Equipment;
using Game.Features.Attributes;
using Game.Features.EquipmentBlueprints;
using Game.Utilities;

namespace Game.Features.Equipment;

public class EquipmentGenerator : IEquipmentGenerator
{
    private readonly IEquipmentTemplateMongoRepository equipmentTemplateMongoRepository;

    public EquipmentGenerator(IEquipmentTemplateMongoRepository equipmentTemplateMongoRepository) =>
        this.equipmentTemplateMongoRepository = equipmentTemplateMongoRepository;
    
    public async Task<Result<EquipmentBase>> GenerateEquipment(string equipmentType)
    {
        var equipmentResult = EquipmentFactory.CreateDrop(equipmentType);
        
        if (equipmentResult.IsFailure)
        {
            return equipmentResult;
        }
        
        var templateResult = await equipmentTemplateMongoRepository.GetByEquipmentIdAsync(equipmentResult.Value.EquipmentId);

        if (templateResult.IsFailure)
        {
            return templateResult.AsError<EquipmentBase>();
        }

        var template = templateResult.Value;
        
        int attributeCount = GetWeightedRandom(template.AttributeCountWeights);

        for (int i = 0; i < attributeCount; i++)
        {
            if (template.AttributeRanges.Count == 0) break;

            int index = RandomHelper.Instance.Next(template.AttributeRanges.Count);

            var range = template.AttributeRanges[index];
            template.AttributeRanges.RemoveAt(index);

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
            if (randomValue < entry.Value)
            {
                return Convert.ToInt32(entry.Key,CultureInfo.InvariantCulture);
            }
            randomValue -= entry.Value;
        }

        return Convert.ToInt32(attributeCountWeights.Keys.First(),CultureInfo.InvariantCulture); 
    }
}
