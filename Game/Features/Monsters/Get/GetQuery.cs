using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Features.Monsters.Get;

public sealed record GetQuery(string MonsterName) : IRequest<Result<Monster>>;
