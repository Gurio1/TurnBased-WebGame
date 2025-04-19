using Game.Core.Abilities;
using Game.Core.Common;
using Game.Core.Models;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Features.Monsters.GetMonster;

public sealed class QueryHandler : IRequestHandler<Query, Result<Monster>>
{
    private readonly IMongoDatabase mongoDatabase;
    private readonly IOptions<MongoSettings> settings;
    private readonly IMongoCollection<Monster> collection;
    
    public QueryHandler(IMongoClient mongoClient,
        IOptions<MongoSettings> settings,IMongoCollectionProvider<Monster> provider)
    {
        this.settings = settings;
        collection = provider.Collection;
        
        mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);
    }
    public async Task<Result<Monster>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!settings.Value.CollectionNames.TryGetValue(nameof(Ability), out string? collName))
            return Result<Monster>.Failure($"No mongo collection name configured for type '{nameof(Ability)}'");
        
        try
        {
            var lookupResult = await collection.AsQueryable()
                .Where(p => p.Name == request.MonsterName)
                .Lookup(mongoDatabase.GetCollection<Ability>(collName),
                    (m, ab) => ab
                        .Where(ability => m.AbilityIds.Contains(ability.Id)))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            
            lookupResult.Local.Abilities = lookupResult.Results.ToArray();
            
            return lookupResult.Local is null
                ? Result<Monster>.NotFound($"Unable to retrieve monster with name '{request.MonsterName}'")
                : Result<Monster>.Success(lookupResult.Local);
        }
        catch (Exception e)
        {
            return Result<Monster>.Failure(e.Message);
        }
    }
}
