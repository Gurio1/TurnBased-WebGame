using Game.Core;
using Game.Core.Abilities;
using Game.Core.Models;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Features.Monsters;

public class MonstersMongoRepository : IMonstersMongoRepository
{
    private readonly IOptions<MongoSettings> mongoDatabaseSettings;
    private readonly IMongoCollection<Monster> monstersCollection;
    private readonly IMongoDatabase mongoDatabase;

    //TODO: Implement error handling(or use EF Core)
    public MonstersMongoRepository(IMongoClient mongoClient,
        IOptions<MongoSettings> mongoDatabaseSettings)
    {
        this.mongoDatabaseSettings = mongoDatabaseSettings;

        mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        monstersCollection = mongoDatabase.GetCollection<Monster>(
            mongoDatabaseSettings.Value.MonstersCollectionName);
        
    }

    public async Task<Result<Monster>> GetByName(string monsterName)
    {
        try
        {
            var result = await monstersCollection.Find(a => a.Name == monsterName).FirstOrDefaultAsync();

            return result is null
                ? Result<Monster>.NotFound($"Unable to retrieve monster with name '{monsterName}'")
                : Result<Monster>.Success(result);
        }
        catch (Exception e)
        {
            return Result<Monster>.Failure(e.Message);
        }
    }
    public async Task<Result<Monster>> GetByNameWithAbilities(string monsterName)
    {
        try
        {
            var lookupResult =  await monstersCollection.AsQueryable()
                .Where(p => p.Name == monsterName)
                .Lookup(mongoDatabase.GetCollection<Ability>(mongoDatabaseSettings.Value.AbilitiesCollectionName),
                    (m, ab) => ab
                        .Where(ability => m.AbilityIds.Contains(ability.Id))).FirstOrDefaultAsync();

            lookupResult.Local.Abilities = lookupResult.Results.ToArray();

            return lookupResult.Local is null
                ? Result<Monster>.NotFound($"Unable to retrieve monster with name '{monsterName}'")
                : Result<Monster>.Success(lookupResult.Local);
        }
        catch (Exception e)
        {
            return Result<Monster>.Failure(e.Message);
        }
    }
    public async Task<Result<Monster>> CreateAsync(Monster newMonster)
    {
        try
        {
            await monstersCollection.InsertOneAsync(newMonster);
            return Result<Monster>.Success(newMonster);
        }
        catch (Exception e)
        {
            return Result<Monster>.Failure(e.Message);
        }
    }

    public async Task<ResultWithoutValue> RemoveAsync(string monsterName)
    {
        if (string.IsNullOrEmpty(monsterName))
        {
            return ResultWithoutValue.Failure(new CustomError("400","Monster name can not be empty"));
        }

        try
        {
           await monstersCollection.DeleteOneAsync(x => x.Name == monsterName);
           return ResultWithoutValue.Success();
        }
        catch (Exception e)
        {
            return ResultWithoutValue.Failure(new CustomError("500", e.Message));
        }
        
    }
        
}
