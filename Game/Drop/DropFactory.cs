using System.Linq.Expressions;
using System.Reflection;
using Game.Core.Abilities;
using Game.Drop;

namespace Game.Core;

public static class DropFactory
{
    private static readonly Dictionary<string, Func<IDropable>> _dropFactory = new();

    static DropFactory()
    {
        var abilityTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } 
                        && typeof(IDropable).IsAssignableFrom(t));

        foreach (var type in abilityTypes)
        {
            var constructor = Expression.New(type);
            var lambda = Expression.Lambda<Func<IDropable>>(constructor);
            _dropFactory[type.Name] = lambda.Compile();
        }
    }

    public static IDropable CreateDrop(string typeName)
    {
        if (_dropFactory.TryGetValue(typeName, out var factoryMethod))
        {
            return factoryMethod();
        }

        throw new InvalidOperationException($"Drop type '{typeName}' not found.");
    }
}