using Game.Core.Models;
using Game.Persistence.Mongo;
using Game.SharedKernel.Results;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Persistence.Repositories;

public sealed class MonsterMongoRepository : IMonsterRepository
{
    private readonly IMongoCollection<Monster> collection;

    public MonsterMongoRepository(IMongoCollectionProvider provider)
    {
        collection = provider.GetCollection<Monster>();
        CreateIndexes();
    }

    public async Task<Result<IReadOnlyCollection<Monster>>> GetAll(CancellationToken ct = default)
    {
        var monsters = await collection.Find(FilterDefinition<Monster>.Empty).ToListAsync(ct);
        return Result<IReadOnlyCollection<Monster>>.Success(monsters);
    }

    public async Task<Result<Monster>> GetByName(string monsterName, CancellationToken ct = default)
    {
        var result = await collection
            .AsQueryable()
            .Where(monster => monster.Name == monsterName)
            .FirstOrDefaultAsync(ct);

        return result is null
            ? Result<Monster>.NotFound($"Monster '{monsterName}' not found")
            : Result<Monster>.Success(result);
    }

    public async Task<Result<Monster>> Create(Monster monster, CancellationToken ct = default)
    {
        try
        {
            await collection.InsertOneAsync(monster, cancellationToken: ct);
            return Result<Monster>.Success(monster);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Result<Monster>.Invalid($"Monster '{monster.Name}' already exists.");
        }
        catch (Exception ex)
        {
            return Result<Monster>.Failure(ex.Message);
        }
    }

    public async Task<ResultWithoutValue> Update(string currentMonsterName, Monster monster, CancellationToken ct = default)
    {
        try
        {
            var result = await collection.ReplaceOneAsync(
                item => item.Name == currentMonsterName,
                monster,
                cancellationToken: ct);

            if (result.MatchedCount == 0)
            {
                return ResultWithoutValue.NotFound($"Monster '{currentMonsterName}' was not found.");
            }

            return ResultWithoutValue.Success();
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return ResultWithoutValue.Invalid($"Monster '{monster.Name}' already exists.");
        }
    }

    public async Task<ResultWithoutValue> Delete(string monsterName, CancellationToken ct = default)
    {
        var deleteResult = await collection.DeleteOneAsync(monster => monster.Name == monsterName, ct);

        return deleteResult.DeletedCount == 0
            ? ResultWithoutValue.NotFound($"Can't delete monster with name '{monsterName}'. Not found.")
            : ResultWithoutValue.Success();
    }

    public Task<bool> ExistsByName(string monsterName, string? excludedMonsterName = null, CancellationToken ct = default)
    {
        var filter = Builders<Monster>.Filter.Eq(item => item.Name, monsterName);

        if (!string.IsNullOrWhiteSpace(excludedMonsterName))
        {
            filter &= Builders<Monster>.Filter.Ne(item => item.Name, excludedMonsterName);
        }

        return collection.Find(filter).AnyAsync(ct);
    }

    private void CreateIndexes()
    {
        var indexKeys = Builders<Monster>.IndexKeys.Ascending(item => item.Name);
        var options = new CreateIndexOptions { Unique = true, Name = "ux_monsters_name" };
        var model = new CreateIndexModel<Monster>(indexKeys, options);
        collection.Indexes.CreateOne(model);
    }
}
