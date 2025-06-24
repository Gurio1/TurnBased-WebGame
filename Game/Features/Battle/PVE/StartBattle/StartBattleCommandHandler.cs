using Game.Application.SharedKernel;
using Game.Core.Abilities;
using Game.Core.Battle;
using Game.Core.Battle.PVE;
using Game.Core.Models;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Persistence.Mongo;
using Game.Persistence.Requests;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Features.Battle.PVE.StartBattle;

public sealed class StartBattleCommandHandler : IRequestHandler<StartBattleCommand, Result<PveBattle>>
{
    private readonly IDispatcher dispatcher;
    private readonly GetMonsterQuery getMonsterQuery;
    private readonly IBattleRepository battleRepository;
    private readonly IMongoCollectionProvider mongoProvider;
    
    public StartBattleCommandHandler(IMongoCollectionProvider provider, IDispatcher dispatcher,
        GetMonsterQuery getMonsterQuery, IBattleRepository battleRepository)
    {
        this.dispatcher = dispatcher;
        this.getMonsterQuery = getMonsterQuery;
        this.battleRepository = battleRepository;
        mongoProvider = provider;
    }
    
    public async Task<Result<PveBattle>> Handle(StartBattleCommand request, CancellationToken cancellationToken)
    {
        var monsterResult = await getMonsterQuery.GetByNameAsync(request.MonsterName, cancellationToken);
        
        var playerResult = await GetCombatPlayer(request.PlayerId, cancellationToken);
        
        var battleResult = PveBattle.Create(playerResult, monsterResult);
        
        if (battleResult.IsFailure)
        {
            return battleResult;
        }
        
        //TODO : How to do ACID atomicity???Actually this is a big problem or no? YAGNI?Is ACID applicable here?Can i possibly have that kind of error??
        var saveResult = await battleRepository.Save(battleResult.Value);
        
        if (saveResult.IsFailure)
            return Result<PveBattle>.CustomError(saveResult.Error);
        
        var updateResult = await SetBattleIdToThePlayer(battleResult.Value.Id, playerResult.Value.Id, cancellationToken);
        
        return updateResult.IsFailure
            ? Result<PveBattle>.CustomError(updateResult.Error)
            : Result<PveBattle>.Success(battleResult.Value);
    }
    
    private async Task<Result<CombatPlayer>> GetCombatPlayer(string playerId, CancellationToken cancellationToken)
    {
        var combatPlayer = await mongoProvider.GetCollection<GamePlayer>().AsQueryable()
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
                Abilities = p.Results.ToArray()
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return combatPlayer is null
            ? Result<CombatPlayer>.NotFound($"Player with id '{playerId}'was not found")
            : Result<CombatPlayer>.Success(combatPlayer);
    }
    
    
    //TODO: Well,do i need to set it right now,after battle creation or i should set it after cache expires?
    //Should Player hold battle id at all?
    private async Task<ResultWithoutValue> SetBattleIdToThePlayer(string battleId, string playerId,
        CancellationToken ct)
    {
        var update = Builders<GamePlayer>.Update.Set(p => p.BattleId, battleId);
        
        var updateResult = await mongoProvider.GetCollection<GamePlayer>()
            .UpdateOneAsync(p => p.Id == playerId, update, cancellationToken: ct);
        
        return updateResult.ModifiedCount == 0
            ? ResultWithoutValue.Failure($"Can not update player battle id - player id '{playerId}'")
            : ResultWithoutValue.Success();
    }
}
