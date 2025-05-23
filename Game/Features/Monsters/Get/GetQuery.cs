using Game.Application.SharedKernel;
using Game.Core.Models;

namespace Game.Features.Monsters.Get;

public sealed record GetQuery(string MonsterName) : IRequest<Result<Monster>>;
