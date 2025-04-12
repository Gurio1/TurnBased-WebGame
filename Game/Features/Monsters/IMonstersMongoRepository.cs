using Game.Core;
using Game.Core.Models;

namespace Game.Features.Monsters;

public interface IMonstersMongoRepository
{
    public Task<Result<Monster>> GetByName(string monsterName);
    public Task<Result<Monster>> GetByNameWithAbilities(string monsterName);
    public Task<Result<Monster>> CreateAsync(Monster newMonster);
    public Task<ResultWithoutValue> RemoveAsync(string monsterName);
}