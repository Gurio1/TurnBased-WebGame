using Game.Services;

namespace Game.Core.Equipment;

public class EquipmentGenerator
{
    private readonly EquipmentTemplateService _equipmentTemplateService;
    private readonly EquipmentService _equipmentService;

    public EquipmentGenerator(EquipmentTemplateService equipmentTemplateService,EquipmentService equipmentService)
    {
        _equipmentTemplateService = equipmentTemplateService;
        _equipmentService = equipmentService;
    }
    
    public async Task<EquipmentBase> GenerateEquipment(string equipmentId)
    {
        var equipment = await _equipmentService.GetById(equipmentId);
        
        if (equipment is null)
        {
            throw new Exception($"Equipment with Id: {equipmentId} doesnt exist");
        }
        
        var template = await _equipmentTemplateService.GetByEquipmentIdAsync(equipmentId);

        if (template is null)
        {
            throw new Exception($"Equipment with Id: {equipmentId} doesnt have template");
        }
        
        var attributeCount = GetWeightedRandom(template.AttributeCountWeights);

        for (int i = 0; i < attributeCount; i++)
        {
            if (template.AttributeRanges.Count == 0) break;

            int index = RandomHelper.Instance.Next(template.AttributeRanges.Count);

            var range = template.AttributeRanges[index];
            template.AttributeRanges.RemoveAt(index);
            
            range.Attribute.Value = (float)Math.Round(RandomHelper.NextFloat(range.MinValue,range.MaxValue), 2);
            
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