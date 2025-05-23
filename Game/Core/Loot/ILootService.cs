using Game.Application.SharedKernel;
using Game.Core.Models;
using Game.Core.PlayerProfile;

namespace Game.Core.Loot;

public interface ILootService
{
    Task<Result<Item?>> GenerateDrop(Monster monster);
}
