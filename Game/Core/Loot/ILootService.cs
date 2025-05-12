using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Core.Loot;

public interface ILootService
{
    Task<Result<Item?>> GenerateDrop(Monster monster);
}
