using Game.Application.SharedKernel;
using Game.Core.Abilities;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Persistence.Repositories;

public sealed class PlayerMongoRepository : IPlayerRepository
{
    private readonly IMongoCollectionProvider provider;
    
    public PlayerMongoRepository(IMongoCollectionProvider provider) =>
        this.provider = provider;
    
    public async Task<Result<GamePlayer>> GetById(string playerId, CancellationToken ct = default)
    {
        var player = await provider.GetCollection<GamePlayer>()
            .Find(p => p.Id == playerId)
            .FirstOrDefaultAsync(ct);
        
        return player is null
            ? Result<GamePlayer>.NotFound($"Player with id '{playerId}' does not exist")
            : Result<GamePlayer>.Success(player);
    }
    
    public async Task<Result<GamePlayer>> GetByIdWithAbilities(string playerId, CancellationToken ct = default)
    {
        var lookupResult = await provider.GetCollection<GamePlayer>()
            .AsQueryable()
            .Where(p => p.Id == playerId)
            .WithAbilities(provider.GetCollection<Ability>())
            .FirstOrDefaultAsync(ct);
        
        if (lookupResult.Local is null)
            return Result<GamePlayer>.NotFound($"Unable to retrieve player with id '{playerId}'");
        
        lookupResult.Local.Abilities = lookupResult.Results.ToList();
        
        return Result<GamePlayer>.Success(lookupResult.Local);
    }
}
