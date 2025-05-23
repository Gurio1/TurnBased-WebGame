using Game.Application.SharedKernel;
using Game.Core.Battle.PVE;

namespace Game.Features.Battle.PVE.StartBattle;

public record StartBattleCommand(string MonsterName, string PlayerId) : IRequest<Result<PveBattle>>;
