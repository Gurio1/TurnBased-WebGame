using Game.Core.Equipment;
using Game.Core.Equipment.Generation;
using Game.Core.Equipment.Generation.Steps;
using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Application.Equipment.Generation;

public sealed class EquipmentGenerator(IEquipmentBlueprintRepository blueprintRepository) : IEquipmentGenerator
{
    public async Task<Result<EquipmentBase>> GenerateEquipment(string equipmentType)
    {
        var equipmentResult = EquipmentFactory.CreateEmpty(equipmentType);
        
        if (equipmentResult.IsFailure)
            return equipmentResult;
        
        var blueprintResult = await blueprintRepository.GetByEquipmentId(equipmentResult.Value.EquipmentId);
        
        if (blueprintResult.IsFailure)
            return blueprintResult.AsError<EquipmentBase>();
        
        var blueprintAttributes = new BlueprintAttributes
        {
            Stats = blueprintResult.Value.Stats
                .Select(stat => new BlueprintStatRange
                {
                    StatKey = stat.StatKey,
                    MinValue = stat.MinValue,
                    MaxValue = stat.MaxValue
                })
                .ToList(),
            AttributeCountWeights = blueprintResult.Value.AttributeCountWeights
        };
        
        var context = new EquipmentGenerationContext(equipmentResult.Value) { Blueprint = blueprintAttributes };
        
        var pipeline = new EquipmentGenerationPipeline()
            .AddStep(new ApplyBlueprintStatsStep());
        
        var generatedEquipment = pipeline.Execute(context);
        
        return Result<EquipmentBase>.Success(generatedEquipment);
    }
}

