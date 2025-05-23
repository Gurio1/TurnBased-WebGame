using Game.Application.SharedKernel;
using Game.Core.PlayerProfile;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Persistence.Requests;

public sealed class UpdatePlayerAfterEquipmentInteraction
{
    private readonly IMongoCollection<Player> collection;
    
    public UpdatePlayerAfterEquipmentInteraction(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<Player>();
    
    public async Task<Result<Player>> Update(Player player, CancellationToken ct = default)
    {
        var updateDef = Builders<Player>.Update
            .Set(p => p.Equipment, player.Equipment)
            .Set(p => p.Inventory, player.Inventory)
            .Set(p => p.Stats, player.Stats);
        
        var result =
            await collection.UpdateOneAsync(p => p.Id == player.Id, updateDef, cancellationToken: ct);
        
        if (result.MatchedCount == 0) return Result<Player>.NotFound($"Player with id '{player.Id}' not found");
        
        return result.ModifiedCount > 0
            ? Result<Player>.Success(player)
            : Result<Player>.Failure($"Unable to modify player with id '{player.Id}'");
    }
}
