using Game.Application.SharedKernel;
using Game.Core.Battle.PVE;

namespace Game.Features.Battle.PVE.GetBattle;

public record GetBattleQuery(string BattleId) : IRequest<Result<PveBattle>>;
