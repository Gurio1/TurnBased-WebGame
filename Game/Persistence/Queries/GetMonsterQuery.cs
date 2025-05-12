using Game.Core.Abilities;
using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Persistence.Queries;

public sealed class GetMonsterQuery
{
    private readonly IMongoCollectionProvider provider;
    
    public GetMonsterQuery(IMongoCollectionProvider provider) =>
        this.provider = provider;
    
    public async Task<Result<Monster>> GetByNameAsync(string monsterName, CancellationToken cancellationToken = default)
    {
        var lookupResult = await provider.GetCollection<Monster>().AsQueryable()
            .Where(p => p.Name == monsterName)
            .WithAbilities(provider.GetCollection<Ability>())
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (lookupResult.Local is null)
            return Result<Monster>.NotFound($"Monster '{monsterName}' not found");
        
        lookupResult.Local.Abilities = lookupResult.Results.ToArray();
        
        return Result<Monster>.Success(lookupResult.Local);
    }
}
