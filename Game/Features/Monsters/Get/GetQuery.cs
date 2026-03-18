using Game.Core.Models;
using Game.SharedKernel;

namespace Game.Features.Monsters.Get;

public sealed record GetQuery(string MonsterName) : IRequest<Result<Monster>>;
