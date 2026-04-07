using Game.Contracts;
using Game.Core.PlayerProfile.Aggregates;
using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Results;

namespace Game.Application.Players;

public interface IPlayerService
{
    Task<Result<GamePlayer>> GetById(string playerId, CancellationToken ct = default);
    Task<Result<PlayerViewModel>> EquipItem(string playerId, string itemId, CancellationToken ct = default);
    Task<Result<PlayerViewModel>> SellItem(string playerId, string itemId, CancellationToken ct = default);
    Task<Result<PlayerViewModel>> UnequipItem(string playerId, string equipmentSlot, CancellationToken ct = default);
    Task<Result<string>> Create(CancellationToken ct = default);
    Task<ResultWithoutValue> Delete(string playerId, CancellationToken ct = default);
    Task<Result<BattleResolveResponse>> ResolveBattle(string playerId, BattleResolveRequest request, CancellationToken ct = default);
}
