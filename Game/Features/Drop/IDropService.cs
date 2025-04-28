using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Features.Drop;

public interface IDropService
{
    Task<Result<Item?>> GenerateDrop(Monster monster);
}
