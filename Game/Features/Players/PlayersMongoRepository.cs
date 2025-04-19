using Game.Core;
using Game.Core.Abilities;
using Game.Core.Common;
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
    private readonly IOptions<MongoSettings> mongoDatabaseSettings;
    private readonly IMongoCollection<Player> playersCollection;
    private readonly IMongoDatabase mongoDatabase;

    public PlayersMongoRepository(IMongoClient mongoClient,
        IOptions<MongoSettings> mongoDatabaseSettings)
    {
        this.mongoDatabaseSettings = mongoDatabaseSettings;

        mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        playersCollection = mongoDatabase.GetCollection<Player>(
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
            var result = await playersCollection.Find(a => a.Id == playerId).FirstOrDefaultAsync();
            
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
            await playersCollection.UpdateOneAsync(p => p.Id == playerId, updateDef);

            return ResultWithoutValue.Success();
        }
        catch (Exception e)
        {
            return ResultWithoutValue.Failure(new CustomError("500",e.Message));
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
            var lookupResult =  await playersCollection.AsQueryable()
                .Where(p => p.Id == playerId)
                .Lookup(mongoDatabase.GetCollection<Ability>(mongoDatabaseSettings.Value.AbilitiesCollectionName),
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
            await playersCollection.InsertOneAsync(newPlayer);
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
            await playersCollection.ReplaceOneAsync(x => x.Id == updatedPlayer.Id, updatedPlayer);
            return ResultWithoutValue.Success();
        }
        catch (Exception e)
        {
            return ResultWithoutValue.Failure(new CustomError("500", e.Message));
        }
    }
        

    public async Task RemoveAsync(string id) =>
        await playersCollection.DeleteOneAsync(x => x.Id == id);
}
