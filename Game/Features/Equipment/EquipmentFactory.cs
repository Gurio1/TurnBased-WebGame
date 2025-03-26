using System.Linq.Expressions;
using System.Reflection;
using Game.Core.Equipment;

namespace Game.Features.Equipment;

public static class EquipmentFactory
{
    private static readonly Dictionary<string, Func<EquipmentBase>> _equipmentFactory = new();

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
            _equipmentFactory[type.Name] = lambda.Compile();
        }
    }

    public static EquipmentBase CreateDrop(string typeName)
    {
        if (_equipmentFactory.TryGetValue(typeName, out var factoryMethod))
        {
            return factoryMethod();
        }

        throw new InvalidOperationException($"Drop type '{typeName}' not found.");
    }
}