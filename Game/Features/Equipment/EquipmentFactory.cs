using System.Linq.Expressions;
using System.Reflection;
using Game.Core;
using Game.Core.Common;
using Game.Core.Equipment;

namespace Game.Features.Equipment;

public static class EquipmentFactory
{
    private static readonly Dictionary<string, Func<EquipmentBase>> equipmentFactory = new();

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
            equipmentFactory[type.Name] = lambda.Compile();
        }
    }

    public static Result<EquipmentBase> CreateDrop(string typeName) =>
        equipmentFactory.TryGetValue(typeName, out var factoryMethod)
            ? Result<EquipmentBase>.Success(factoryMethod())
            : Result<EquipmentBase>.NotFound($"Equipment type '{typeName}' not found.");
}
