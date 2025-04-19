using Game.Core.Common;
using Game.Core.Models;

namespace Game.Features.Monsters.CreateMonster;

public sealed record Command(Monster Monster)
    : IRequest<Result<Monster>>;
