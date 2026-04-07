using Game.Core.Models;
using Game.SharedKernel.Results;

namespace Game.Application.Monsters;

public interface IMonsterService
{
    Task<Result<IReadOnlyCollection<Monster>>> GetAll(CancellationToken ct = default);
    Task<Result<Monster>> GetByName(string monsterName, CancellationToken ct = default);
    Task<Result<IReadOnlyCollection<ItemCatalogEntry>>> GetItemCatalog(CancellationToken ct = default);
    Task<Result<IReadOnlyCollection<MonsterAbilityCatalogEntry>>> GetAbilityCatalog(CancellationToken ct = default);
    Task<Result<IReadOnlyCollection<MonsterStatCatalogEntry>>> GetStatCatalog(CancellationToken ct = default);
    Task<Result<Monster>> Create(SaveMonsterModel model, CancellationToken ct = default);
    Task<ResultWithoutValue> Update(string currentMonsterName, SaveMonsterModel model, CancellationToken ct = default);
    Task<ResultWithoutValue> Delete(string monsterName, CancellationToken ct = default);
}
