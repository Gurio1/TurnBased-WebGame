using Game.Core.Common;
using Game.Core.Models;

namespace Game.Features.Monsters.GetMonster;

public sealed record Query(string MonsterName) : IRequest<Result<Monster>>;
