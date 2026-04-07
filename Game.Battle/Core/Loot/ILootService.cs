using Game.Battle.Core.Models;
using Game.SharedKernel.Results;

namespace Game.Battle.Core.Loot;

public interface ILootService
{
    Task<Result<LootResult?>> GenerateDrop(Monster monster);
}
