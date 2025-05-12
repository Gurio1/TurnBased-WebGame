using Game.Core;
using Game.Core.Abilities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Persistence.Mongo;

public static class MongoQueryExtensions
{
    public static IQueryable<LookupResult<T, Ability>> WithAbilities<T>(this IQueryable<T> query,
        IMongoCollection<Ability> abilityCollection) where T : IHasAbilityIds =>
        query.Lookup(abilityCollection,
            (model, ability) => ability.Where(a => model.AbilityIds.Contains(a.Id)));
}
