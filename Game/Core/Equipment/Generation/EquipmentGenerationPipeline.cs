using Game.Utilities;

namespace Game.Core.Equipment.Generation;

public sealed class EquipmentGenerationPipeline
{
    private readonly List<IEquipmentPipelineStep> steps = [];
    
    public EquipmentGenerationPipeline AddStep(IEquipmentPipelineStep step)
    {
        steps.Add(step);
        return this;
    }
    
    public EquipmentBase Execute(EquipmentGenerationContext context)
    {
        foreach (var step in steps)
        {
            if (step.CanApply(context) && RandomHelper.Instance.NextDouble() <= step.Weight)
            {
                step.Apply(context);
            }
        }
        
        return context.Equipment;
    }
}
