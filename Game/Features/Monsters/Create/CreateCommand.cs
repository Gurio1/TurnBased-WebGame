using Game.Core.Models;
using Game.SharedKernel;

namespace Game.Features.Monsters.Create;

public sealed record CreateCommand(Monster Monster)
    : IRequest<Result<Monster>>;
