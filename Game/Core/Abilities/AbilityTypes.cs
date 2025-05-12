using System.Reflection;

namespace Game.Core.Abilities;

public static class AbilityTypes
{
    public static IEnumerable<Type> Discover() =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(Ability).IsAssignableFrom(t));
}
