using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Features.Monsters.CreateMonster;

public sealed record CreateCommand(Monster Monster)
    : IRequest<Result<Monster>>;
