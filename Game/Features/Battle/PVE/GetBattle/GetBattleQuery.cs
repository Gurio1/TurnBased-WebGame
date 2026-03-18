using Game.Core.Battle.PVE;
using Game.SharedKernel;

namespace Game.Features.Battle.PVE.GetBattle;

public record GetBattleQuery(string BattleId) : IRequest<Result<PveBattle>>;
