using Game.Core.Common;

namespace Game.Features.Monsters.DeleteMonster;

public sealed record DeleteCommand(string MonsterName) : IRequest<ResultWithoutValue>;
