using Game.Core.Abilities;

namespace Game.Features.Abilities;

public interface IAbilityService
{
    Task<Ability> GetById(string id);
    Task<List<Ability>> GetByIdsAsync(List<string> ids);
    Task Save(Ability ability);
}