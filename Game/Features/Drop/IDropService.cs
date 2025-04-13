using Game.Core;
using Game.Core.Models;

namespace Game.Features.Drop;

public interface IDropService
{
    Task<Result<Item?>> GenerateDrop(Monster monster);
}
