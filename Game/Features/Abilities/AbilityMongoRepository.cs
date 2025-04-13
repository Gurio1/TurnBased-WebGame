using Game.Core;
using Game.Core.Abilities;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Game.Features.Abilities;

public class AbilityMongoRepository : IAbilityMongoRepository
{
    private readonly IMongoCollection<Ability> abilitiesCollection;

    //TODO: Implement error handling(or use EF Core)
    public AbilityMongoRepository(IMongoClient mongoClient,
        IOptions<MongoSettings> mongoDatabaseSettings)
    {

        var mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        abilitiesCollection = mongoDatabase.GetCollection<Ability>(
            mongoDatabaseSettings.Value.AbilitiesCollectionName);

    }
    public async Task<Result<Ability>> GetById(string id)
    {
        var ability = await abilitiesCollection.Find(a => a.Id == id).FirstOrDefaultAsync();

        return ability is null ? Result<Ability>.NotFound($"Can not find ability with id - {id}")
            : Result<Ability>.Success(ability);
    }

    public async Task<Result<List<Ability>>> GetByIdsAsync(List<string> ids)
    {
        var filter = Builders<Ability>.Filter.In(a => a.Id, ids);
        var abilities = await abilitiesCollection.Find(filter).ToListAsync();

        return abilities is null
            ? Result<List<Ability>>.NotFound($"Can not find ability with id - {string.Join(',',ids)}")
            : Result<List<Ability>>.Success(abilities);
    }

    public async Task<ResultWithoutValue> Save(Ability ability)
    {
        try
        {
            await abilitiesCollection.InsertOneAsync(ability);
            return ResultWithoutValue.Success();
        }
        catch (Exception e)
        {
            return ResultWithoutValue.Failure(new CustomError("500",e.Message));
        }
    }
}
