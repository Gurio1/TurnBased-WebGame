using Game.Application.SharedKernel;
using Game.Core.PlayerProfile.Aggregates;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Players.Sell;

public sealed class UpdatePlayerAfterSellInteraction
{
    private readonly IMongoCollection<GamePlayer> collection;
    
    public UpdatePlayerAfterSellInteraction(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<GamePlayer>();
    
    public async Task<Result<GamePlayer>> Update(GamePlayer gamePlayer, CancellationToken ct = default)
    {
        var updateDef = Builders<GamePlayer>.Update
            .Set(p => p.Inventory, gamePlayer.Inventory);
        
        var result =
            await collection.UpdateOneAsync(p => p.Id == gamePlayer.Id, updateDef, cancellationToken: ct);
        
        if (result.MatchedCount == 0) return Result<GamePlayer>.NotFound($"Player with id '{gamePlayer.Id}' not found");
        
        return result.ModifiedCount > 0
            ? Result<GamePlayer>.Success(gamePlayer)
            : Result<GamePlayer>.Failure($"Unable to modify player with id '{gamePlayer.Id}'");
    }
}
