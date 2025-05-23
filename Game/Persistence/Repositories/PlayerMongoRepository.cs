using Game.Application.SharedKernel;
using Game.Core.Abilities;
using Game.Core.PlayerProfile;
using Game.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Persistence.Repositories;

public sealed class PlayerMongoRepository : IPlayerRepository
{
    private readonly IMongoCollectionProvider provider;
    
    public PlayerMongoRepository(IMongoCollectionProvider provider) =>
        this.provider = provider;
    
    public async Task<Result<Player>> GetById(string playerId, CancellationToken ct = default)
    {
        var player = await provider.GetCollection<Player>()
            .Find(p => p.Id == playerId)
            .FirstOrDefaultAsync(ct);
        
        return player is null
            ? Result<Player>.NotFound($"Player with id '{playerId}' does not exist")
            : Result<Player>.Success(player);
    }
    
    public async Task<Result<Player>> GetByIdWithAbilities(string playerId, CancellationToken ct = default)
    {
        var lookupResult = await provider.GetCollection<Player>()
            .AsQueryable()
            .Where(p => p.Id == playerId)
            .WithAbilities(provider.GetCollection<Ability>())
            .FirstOrDefaultAsync(ct);
        
        if (lookupResult.Local is null)
            return Result<Player>.NotFound($"Unable to retrieve player with id '{playerId}'");
        
        lookupResult.Local.Abilities = lookupResult.Results.ToList();
        
        return Result<Player>.Success(lookupResult.Local);
    }
}
