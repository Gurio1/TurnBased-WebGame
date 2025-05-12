using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Persistence.Repositories;

public sealed class PlayerMongoRepository : IPlayerRepository
{
    private readonly IMongoCollection<Player> collection;
    
    public PlayerMongoRepository(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<Player>();
    
    public async Task<Result<Player>> GetById(string playerId)
    {
        var player = await collection.Find(p => p.Id == playerId).FirstOrDefaultAsync();
        
        return player is null
            ? Result<Player>.NotFound($"Player with id '{playerId}' does not exist")
            : Result<Player>.Success(player);
    }
}
