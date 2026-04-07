using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Persistence.Mongo;
using Game.SharedKernel.Results;
using MongoDB.Driver;

namespace Game.Persistence.Repositories;

public sealed class PlayerMongoRepository(IMongoCollectionProvider provider) : IPlayerRepository
{
    public async Task<Result<GamePlayer>> GetById(string playerId, CancellationToken ct = default)
    {
        var player = await provider.GetCollection<GamePlayer>()
            .Find(existing => existing.Id == playerId)
            .FirstOrDefaultAsync(ct);

        return player is null
            ? Result<GamePlayer>.NotFound($"Player with id '{playerId}' does not exist")
            : Result<GamePlayer>.Success(player);
    }

    public async Task<Result<GamePlayer>> Create(GamePlayer player, CancellationToken ct = default)
    {
        try
        {
            await provider.GetCollection<GamePlayer>().InsertOneAsync(player, cancellationToken: ct);
            return Result<GamePlayer>.Success(player);
        }
        catch (Exception ex)
        {
            return Result<GamePlayer>.Failure(ex.Message);
        }
    }

    public async Task<Result<GamePlayer>> Save(GamePlayer player, CancellationToken ct = default)
    {
        try
        {
            var updateResult = await provider.GetCollection<GamePlayer>()
                .ReplaceOneAsync(existing => existing.Id == player.Id, player, cancellationToken: ct);

            return updateResult.MatchedCount == 0
                ? Result<GamePlayer>.NotFound($"Player with id '{player.Id}' does not exist")
                : Result<GamePlayer>.Success(player);
        }
        catch (Exception ex)
        {
            return Result<GamePlayer>.Failure(ex.Message);
        }
    }

    public async Task<ResultWithoutValue> Delete(string playerId, CancellationToken ct = default)
    {
        var deleteResult = await provider.GetCollection<GamePlayer>()
            .DeleteOneAsync(player => player.Id == playerId, ct);

        return deleteResult.DeletedCount == 0
            ? ResultWithoutValue.NotFound($"Can't delete player with id '{playerId}'. Not found.")
            : ResultWithoutValue.Success();
    }
}
