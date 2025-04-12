using Game.Core;
using Game.Core.Models;
using MongoDB.Driver;

namespace Game.Features.Players;

public interface IPlayersMongoRepository
{
    Task<Result<Player>> GetById(string playerId);
    Task<ResultWithoutValue> UpdateOneAsync(string playerId, UpdateDefinition<Player> updateDef);
    Task<Result<Player>> GetByIdWithAbilities(string playerId);
    Task<Result<Player>> CreateAsync(Player newPlayer);
    Task<ResultWithoutValue> UpdateAsync(Player updatedPlayer);
}