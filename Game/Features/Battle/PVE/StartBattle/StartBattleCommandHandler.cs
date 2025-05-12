using Game.Core.Abilities;
using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Features.Battle.Models;
using Game.Persistence.Mongo;
using Game.Persistence.Queries;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Application.Features.Battle.PVE.StartBattle;

public sealed class StartBattleCommandHandler : IRequestHandler<StartBattleCommand, Result<PveBattle>>
{
    private readonly IDispatcher dispatcher;
    private readonly GetMonsterQuery getMonsterQuery;
    private readonly IMongoCollectionProvider mongoProdiver;
    
    public StartBattleCommandHandler(IMongoCollectionProvider provider,IDispatcher dispatcher,GetMonsterQuery getMonsterQuery)
    {
        this.dispatcher = dispatcher;
        this.getMonsterQuery = getMonsterQuery;
        mongoProdiver = provider;
    }
    public async Task<Result<PveBattle>> Handle(StartBattleCommand request, CancellationToken cancellationToken)
    {
        var monsterResult = await getMonsterQuery.GetByNameAsync(request.MonsterName, cancellationToken);
        
        if (monsterResult.IsFailure)
        {
            return monsterResult.AsError<PveBattle>();
        }
        
        var playerResult = await GetCombatPlayer(request.PlayerId,cancellationToken);
        
        if (playerResult.IsFailure)
        {
            return playerResult.AsError<PveBattle>();
        }
        
        if (playerResult.Value.BattleId is not null)
        {
            return Result<PveBattle>.Invalid("Can't create new battle.Player is already in the battle");
        }
        
        var battle = new PveBattle(playerResult.Value, monsterResult.Value);
        
        playerResult.Value.BattleId = battle.Id;
        
        var saveResult = await dispatcher.DispatchAsync(new SaveBattleInRedisCommand(battle), cancellationToken);
        
        if (saveResult.IsFailure)
        {
            return Result<PveBattle>.CustomError(saveResult.Error);
        }
        
        var update = Builders<Player>.Update.Set(p => p.BattleId, battle.Id);
        
        var updateResult = await mongoProdiver.GetCollection<Player>().
            UpdateOneAsync(p => p.Id == playerResult.Value.Id,update, cancellationToken: cancellationToken);
        
        return updateResult.ModifiedCount == 0
            ? Result<PveBattle>.Failure($"Can not update player battle id - player id '{playerResult.Value.Id}'")
            : Result<PveBattle>.Success(battle);
    }
    
    private async Task<Result<CombatPlayer>> GetCombatPlayer(string playerId,CancellationToken cancellationToken)
    {
        var combatPlayer = await mongoProdiver.GetCollection<Player>().AsQueryable()
            .Where(p => p.Id == playerId)
            .WithAbilities(mongoProdiver.GetCollection<Ability>())
            .Select(p => new CombatPlayer()
            {
                Id = p.Local.Id,
                Stats = p.Local.Stats,
                CharacterType = p.Local.CharacterType,
                Debuffs = p.Local.Debuffs,
                Equipment = p.Local.Equipment,
                OtherInventoryItems = p.Local.Inventory.Where(i => i.Item.ItemType != ItemType.Equipment).ToList(),
                Abilities = p.Results.ToList()
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        return combatPlayer is null
            ? Result<CombatPlayer>.NotFound($"Player with id '{playerId}'was not found")
            : Result<CombatPlayer>.Success(combatPlayer);
    }
}
