using Game.Battle.Core.Battle;
using Game.Battle.Core.Battle.PVE;
using Game.SharedKernel.Results;
using MongoDB.Driver;

namespace Game.Battle.Persistence.Mongo;

public sealed class BattleMongoRepository(IMongoCollectionProvider mongoCollectionProvider) : IBattleReadRepository
{
    public async Task<Result<PveBattle>> GetById(string battleId, CancellationToken ct = default)
    {
        var battle = await mongoCollectionProvider
            .GetCollection<PveBattle>()
            .Find(existing => existing.Id == battleId)
            .FirstOrDefaultAsync(ct);

        return battle is null
            ? Result<PveBattle>.NotFound($"Battle with id '{battleId}' doesn't exist")
            : Result<PveBattle>.Success(battle);
    }
}
