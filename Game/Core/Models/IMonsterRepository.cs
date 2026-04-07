using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core.Models;

public interface IMonsterRepository
{
    Task<Result<IReadOnlyCollection<Monster>>> GetAll(CancellationToken ct = default);
    Task<Result<Monster>> GetByName(string monsterName, CancellationToken ct = default);
    Task<Result<Monster>> Create(Monster monster, CancellationToken ct = default);
    Task<ResultWithoutValue> Update(string currentMonsterName, Monster monster, CancellationToken ct = default);
    Task<ResultWithoutValue> Delete(string monsterName, CancellationToken ct = default);
    Task<bool> ExistsByName(string monsterName, string? excludedMonsterName = null, CancellationToken ct = default);
}
