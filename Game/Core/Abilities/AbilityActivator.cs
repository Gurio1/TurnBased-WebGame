using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Game.Core.Abilities;

//TODO: Should i use Result<>?
//TODO: Do i need this cache field here?
public static class AbilityActivator
{
    private static readonly ConcurrentDictionary<Type, Func<Ability>> cache = new();
    
    public static Ability CreateAbility(Type abilityType)
    {
        ArgumentNullException.ThrowIfNull(abilityType);
        
        if (!typeof(Ability).IsAssignableFrom(abilityType))
            throw new ArgumentException($"Type must inherit from {nameof(Ability)}");
        
        return cache.GetOrAdd(abilityType, t =>
        {
            if (t.GetConstructor(Type.EmptyTypes) == null)
                throw new InvalidOperationException("No parameterless constructor found");
            
            return Expression.Lambda<Func<Ability>>(
                Expression.New(t)).Compile();
        })();
    }
}
