using System.Linq.Expressions;
using System.Reflection;
using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core.Equipment.Generation;

public static class EquipmentFactory
{
    private static readonly Dictionary<string, Func<EquipmentBase>> equipmentFactory = new();
    private static readonly List<EquipmentDefinition> equipmentDefinitions = [];
    
    static EquipmentFactory()
    {
        var equipmentTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(EquipmentBase).IsAssignableFrom(t));
        
        foreach (var type in equipmentTypes)
        {
            var constructor = Expression.New(type);
            var lambda = Expression.Lambda<Func<EquipmentBase>>(constructor);
            var factory = lambda.Compile();
            equipmentFactory[type.Name] = factory;

            var equipment = factory();
            equipmentDefinitions.Add(new EquipmentDefinition(
                type.Name,
                equipment.EquipmentId,
                equipment.Name,
                equipment.Slot,
                equipment.ImageUrl));
        }
    }
    
    public static Result<EquipmentBase> CreateEmpty(string typeName) =>
        equipmentFactory.TryGetValue(typeName, out var factoryMethod)
            ? Result<EquipmentBase>.Success(factoryMethod())
            : Result<EquipmentBase>.NotFound($"Equipment type '{typeName}' not found.");

    public static IReadOnlyList<EquipmentDefinition> GetDefinitions() =>
        equipmentDefinitions
            .OrderBy(definition => definition.Slot)
            .ThenBy(definition => definition.Name)
            .ToList();
}
