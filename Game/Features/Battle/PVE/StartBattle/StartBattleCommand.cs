using Game.Core.Battle.PVE;
using Game.SharedKernel;

namespace Game.Features.Battle.PVE.StartBattle;

public record StartBattleCommand(string MonsterName, string PlayerId) : IRequest<Result<PveBattle>>;
