using Game.Application.SharedKernel;
using Game.Core.Models;

namespace Game.Features.Monsters.Create;

public sealed record CreateCommand(Monster Monster)
    : IRequest<Result<Monster>>;
