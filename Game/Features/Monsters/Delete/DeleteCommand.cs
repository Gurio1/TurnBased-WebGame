using Game.Core.SharedKernel;

namespace Game.Features.Monsters.DeleteMonster;

public sealed record DeleteCommand(string MonsterName) : IRequest<ResultWithoutValue>;
