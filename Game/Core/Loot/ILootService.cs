using Game.Core.Models;
using Game.Core.PlayerProfile;
using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core.Loot;

public interface ILootService
{
    Task<Result<GeneratedMonsterDrop?>> GenerateDrop(Monster monster);
}
