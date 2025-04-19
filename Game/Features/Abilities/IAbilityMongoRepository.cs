using Game.Core;
using Game.Core.Abilities;
using Game.Core.Common;

namespace Game.Features.Abilities;

public interface IAbilityMongoRepository
{
    Task<Result<Ability>> GetById(string id);
    Task<Result<List<Ability>>> GetByIdsAsync(List<string> ids);
    Task<ResultWithoutValue> Save(Ability ability);
}