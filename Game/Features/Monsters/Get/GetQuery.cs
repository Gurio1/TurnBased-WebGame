using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Application.Features.Monsters.GetMonster;

public sealed record GetQuery(string MonsterName) : IRequest<Result<Monster>>;
