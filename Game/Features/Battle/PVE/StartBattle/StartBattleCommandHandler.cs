using Game.Core.Abilities;
using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Features.Battle.Models;
using Game.Persistence.Mongo;
using Game.Persistence.Requests;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Features.Battle.PVE.StartBattle;

public sealed class StartBattleCommandHandler : IRequestHandler<StartBattleCommand, Result<PveBattle>>
{
    private readonly IDispatcher dispatcher;
    private readonly GetMonsterQuery getMonsterQuery;
    private readonly IMongoCollectionProvider mongoProvider;
    
    public StartBattleCommandHandler(IMongoCollectionProvider provider, IDispatcher dispatcher,
        GetMonsterQuery getMonsterQuery)
    {
        this.dispatcher = dispatcher;
        this.getMonsterQuery = getMonsterQuery;
        mongoProvider = provider;
    }
    
    public async Task<Result<PveBattle>> Handle(StartBattleCommand request, CancellationToken cancellationToken)
    {
        var monsterResult = await getMonsterQuery.GetByNameAsync(request.MonsterName, cancellationToken);
        
        if (monsterResult.IsFailure)
            return monsterResult.AsError<PveBattle>();
        
        var playerResult = await GetCombatPlayer(request.PlayerId, cancellationToken);
        
        if (playerResult.IsFailure)
            return playerResult.AsError<PveBattle>();
        
        if (playerResult.Value.BattleId is not null)
            return Result<PveBattle>.Invalid("Can't create new battle.Player is already in the battle");
        
        var battle = new PveBattle(playerResult.Value, monsterResult.Value);
        
        playerResult.Value.BattleId = battle.Id;
        
        //TODO : How to do ACID atomicity???Actually this is a big problem or no? YAGNI?Is ACID applicable here?Can i possibly have that kind of error??
        var saveResult = await dispatcher.DispatchAsync(new SaveBattleInRedisCommand(battle), cancellationToken);
        
        if (saveResult.IsFailure)
            return Result<PveBattle>.CustomError(saveResult.Error);
        
        var updateResult = await SetBattleIdToThePlayer(battle.Id, playerResult.Value.Id, cancellationToken);
        
        return updateResult.IsFailure
            ? Result<PveBattle>.CustomError(updateResult.Error)
            : Result<PveBattle>.Success(battle);
    }
    
    private async Task<Result<CombatPlayer>> GetCombatPlayer(string playerId, CancellationToken cancellationToken)
    {
        var combatPlayer = await mongoProvider.GetCollection<Player>().AsQueryable()
            .Where(p => p.Id == playerId)
            .WithAbilities(mongoProvider.GetCollection<Ability>())
            .Select(p => new CombatPlayer
            {
                Id = p.Local.Id,
                Stats = p.Local.Stats,
                CharacterType = p.Local.CharacterType,
                Debuffs = p.Local.Debuffs,
                Equipment = p.Local.Equipment,
                OtherInventoryItems = p.Local.Inventory.Where(i => i.Item.ItemType != ItemType.Equipment).ToList(),
                Abilities = p.Results.ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return combatPlayer is null
            ? Result<CombatPlayer>.NotFound($"Player with id '{playerId}'was not found")
            : Result<CombatPlayer>.Success(combatPlayer);
    }
    
    private async Task<ResultWithoutValue> SetBattleIdToThePlayer(string battleId,string playerId, CancellationToken ct)
    {
        var update = Builders<Player>.Update.Set(p => p.BattleId, battleId);
        
        var updateResult = await mongoProvider.GetCollection<Player>()
            .UpdateOneAsync(p => p.Id == playerId, update, cancellationToken: ct);
        
        return updateResult.ModifiedCount == 0
            ? ResultWithoutValue.Failure($"Can not update player battle id - player id '{playerId}'")
            : ResultWithoutValue.Success();
    }
}
