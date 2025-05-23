using Game.Application.SharedKernel;

namespace Game.Features.Monsters.Delete;

public sealed record DeleteCommand(string MonsterName) : IRequest<ResultWithoutValue>;
