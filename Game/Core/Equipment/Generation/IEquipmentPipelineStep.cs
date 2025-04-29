namespace Game.Core.Equipment.Generation;

public interface IEquipmentPipelineStep
{
    /// <summary>
    /// Chance from 0.0 to 1.0 for this step to apply.
    /// </summary>
    double Weight { get; }
    
    /// <summary>
    /// Determines if the step should run based on runtime context.
    /// </summary>
    bool CanApply(EquipmentGenerationContext context);
    
    EquipmentBase Apply(EquipmentGenerationContext context);
}
