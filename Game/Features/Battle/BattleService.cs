using Game.Core.Common;
using Game.Features.Battle.Models;
using Game.Features.Players;

namespace Game.Features.Battle;

public class BattleService : IBattleService
{
    private readonly IBattleRepository battleRedisRepository;
    private readonly IPlayersMongoRepository playersMongoRepository;
    
    public BattleService(IBattleRepository battleRedisRepository, IPlayersMongoRepository playersMongoRepository)
    {
        this.battleRedisRepository = battleRedisRepository;
        this.playersMongoRepository = playersMongoRepository;
    }
    
    public async Task<Result<PveBattle>> InitializeBattleForPlayerAsync(string playerId)
    {
        var playerResult = await playersMongoRepository.GetByIdWithAbilities(playerId);
        
        if (playerResult.IsFailure) return playerResult.AsError<PveBattle>();
        
        var player = playerResult.Value;
        
        return player.InBattle()
            ? await battleRedisRepository.GetActiveBattleAsync(player.BattleId!)
            : await battleRedisRepository.CreateBattleAsync(player);
    }
    
    public async Task<Result<PveBattle>> GetActiveBattleAsync(string playerId) =>
        await battleRedisRepository.GetActiveBattleAsync(playerId);
}
