using Game.Core.Equipment;
using Game.Services;

namespace Game.Features.Equipment;

public class EquipmentGenerator
{
    private readonly EquipmentTemplateService _equipmentTemplateService;

    public EquipmentGenerator(EquipmentTemplateService equipmentTemplateService)
    {
        _equipmentTemplateService = equipmentTemplateService;
    }
    
    public async Task<EquipmentBase> GenerateEquipment(string equipmentType)
    {
        var equipment = EquipmentFactory.CreateDrop(equipmentType);
        
        if (equipment is null)
        {
            throw new Exception($"Equipment with type: {equipmentType} doesnt exist");
        }
        
        var template = await _equipmentTemplateService.GetByEquipmentIdAsync(equipment.EquipmentId);

        if (template is null)
        {
            throw new Exception($"Equipment with Id: {equipmentType} doesnt have template");
        }
        
        var attributeCount = GetWeightedRandom(template.AttributeCountWeights);

        for (int i = 0; i < attributeCount; i++)
        {
            if (template.AttributeRanges.Count == 0) break;

            int index = RandomHelper.Instance.Next(template.AttributeRanges.Count);

            var range = template.AttributeRanges[index];
            template.AttributeRanges.RemoveAt(index);

            var randomValue = (float)Math.Round(RandomHelper.NextFloat(range.MinValue, range.MaxValue), 2);
            range.Attribute.Value = MathF.Round(randomValue,1);
            
            equipment.Attributes.Add(range.Attribute);
        }

        return equipment;
    }

    private int GetWeightedRandom(Dictionary<string, double> attributeCountWeights)
    {
        double totalWeight = attributeCountWeights.Values.Sum();
        double randomValue = RandomHelper.Instance.NextDouble() * totalWeight;

        foreach (var entry in attributeCountWeights)
        {
            if (randomValue < entry.Value)
            {
                return Convert.ToInt32(entry.Key);
            }
            randomValue -= entry.Value;
        }

        return Convert.ToInt32(attributeCountWeights.Keys.First()); // Fallback
    }
}