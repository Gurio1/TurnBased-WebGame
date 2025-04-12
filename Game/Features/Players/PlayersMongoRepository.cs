using Game.Core;
using Game.Core.Abilities;
using Game.Core.Models;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Sprache;

namespace Game.Features.Players;

public class PlayersMongoRepository : IPlayersMongoRepository
{
    private readonly IOptions<MongoSettings> _mongoDatabaseSettings;
    private readonly IMongoCollection<Player> _playersCollection;
    private readonly IMongoDatabase _mongoDatabase;

    public PlayersMongoRepository(IMongoClient mongoClient,
        IOptions<MongoSettings> mongoDatabaseSettings)
    {
        _mongoDatabaseSettings = mongoDatabaseSettings;

        _mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        _playersCollection = _mongoDatabase.GetCollection<Player>(
            mongoDatabaseSettings.Value.PlayersCollectionName);
        
    }

    public async Task<Result<Player>> GetById(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            return Result<Player>.Invalid("Player id can't be null");
        }
        try
        {
            var result = await _playersCollection.Find(a => a.Id == playerId).FirstOrDefaultAsync();
            
            return result is null 
                ? Result<Player>.NotFound($"Unable to retrieve player with id '{playerId}'")
                : Result<Player>.Success(result);
        }
        catch (Exception e)
        {
            return Result<Player>.Failure(e.Message);
        }
    }

    public async Task<ResultWithoutValue> UpdateOneAsync(string playerId,UpdateDefinition<Player> updateDef)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            return Result<Player>.Invalid("Player id can't be null");
        }

        try
        {
            await _playersCollection.UpdateOneAsync(p => p.Id == playerId, updateDef);

            return ResultWithoutValue.Success();
        }
        catch (Exception e)
        {
            return ResultWithoutValue.Failure(new Error("500",e.Message));
        }
    }
    
    public async Task<Result<Player>> GetByIdWithAbilities(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            return Result<Player>.Invalid("Player id can't be null");
        }
        
        try
        {
            var lookupResult =  await _playersCollection.AsQueryable()
                .Where(p => p.Id == playerId)
                .Lookup(_mongoDatabase.GetCollection<Ability>(_mongoDatabaseSettings.Value.AbilitiesCollectionName),
                    (pl, ab) => ab
                        .Where(ability => pl.AbilityIds.Contains(ability.Id))).FirstOrDefaultAsync();

            lookupResult.Local.Abilities = lookupResult.Results.ToList();

            return lookupResult.Local is null
                ? Result<Player>.NotFound($"Unable to retrieve player with id '{playerId}'")
                : Result<Player>.Success(lookupResult.Local);
        }
        catch (Exception e)
        {
            return Result<Player>.Failure(e.Message);
        }
    }

    public async Task<Result<Player>> CreateAsync(Player newPlayer)
    {
        try
        {
            await _playersCollection.InsertOneAsync(newPlayer);
            return Result<Player>.Success(newPlayer);
        }
        catch (Exception e)
        {
            return Result<Player>.Failure(e.Message);
        }
    }

    public async Task<ResultWithoutValue> UpdateAsync(Player updatedPlayer)
    {
        try
        {
            await _playersCollection.ReplaceOneAsync(x => x.Id == updatedPlayer.Id, updatedPlayer);
            return ResultWithoutValue.Success();
        }
        catch (Exception e)
        {
            return ResultWithoutValue.Failure(new Error("500", e.Message));
        }
    }
        

    public async Task RemoveAsync(string id) =>
        await _playersCollection.DeleteOneAsync(x => x.Id == id);
}