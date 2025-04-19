using Game.Core.Common;

namespace Game.Features.Monsters.DeleteMonster;

public sealed record Command(string MonsterName) : IRequest<ResultWithoutValue>;
