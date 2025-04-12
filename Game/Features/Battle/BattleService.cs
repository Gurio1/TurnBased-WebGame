using Game.Core;
using Game.Core.Models;
using Game.Features.Battle.Models;
using Game.Features.Players;

namespace Game.Features.Battle;

public class BattleService : IBattleService
{
    private readonly IBattleRepository _battleRedisRepository;
    private readonly IPlayersMongoRepository _playersMongoRepository;

    public BattleService(IBattleRepository battleRedisRepository,IPlayersMongoRepository playersMongoRepository)
    {
        _battleRedisRepository = battleRedisRepository;
        _playersMongoRepository = playersMongoRepository;
    }
    
    public async Task<Result<PveBattle>> InitializeBattleForPlayerAsync(string playerId)
    {
        var playerResult = await _playersMongoRepository.GetByIdWithAbilities(playerId);

        if (playerResult.IsFailure)
        {
            return playerResult.AsError<PveBattle>();
        }
        
        var player = playerResult.Value;
        
        return player.InBattle()
            ? await _battleRedisRepository.GetActiveBattleAsync(player.BattleId!)
            : await _battleRedisRepository.CreateBattleAsync(player);
    }

    public async Task<Result<PveBattle>> GetActiveBattleAsync(string playerId)
    {
        return await _battleRedisRepository.GetActiveBattleAsync(playerId);
    }
}