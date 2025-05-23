using Game.Application.SharedKernel;
using Game.Core.Equipment;
using Game.Core.Equipment.Generation;
using Game.Core.Equipment.Generation.Steps;

namespace Game.Features.Equipment.Generation;

public sealed class EquipmentGenerator : IEquipmentGenerator
{
    private readonly IDispatcher dispatcher;
    
    public EquipmentGenerator(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public async Task<Result<EquipmentBase>> GenerateEquipment(string equipmentType)
    {
        var equipmentResult = EquipmentFactory.CreateEmpty(equipmentType);
        
        if (equipmentResult.IsFailure)
            return equipmentResult;
        
        var blueprintResult = await dispatcher.DispatchAsync(new GetBlueprintQuery(equipmentResult.Value.EquipmentId));
        
        if (blueprintResult.IsFailure)
            return blueprintResult.AsError<EquipmentBase>();
        
        var blueprintAttributes = blueprintResult.Value;
        
        var context = new EquipmentGenerationContext(equipmentResult.Value) { Blueprint = blueprintAttributes };
        
        var pipeline = new EquipmentGenerationPipeline()
            .AddStep(new ApplyBlueprintStatsStep());
        
        var generatedEquipment = pipeline.Execute(context);
        
        return Result<EquipmentBase>.Success(generatedEquipment);
    }
}
