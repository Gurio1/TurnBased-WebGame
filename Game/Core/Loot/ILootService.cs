using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Features.Loot;

public interface ILootService
{
    Task<Result<Item?>> GenerateDrop(Monster monster);
}
