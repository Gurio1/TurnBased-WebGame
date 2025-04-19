using Game.Core.Common;
using Game.Core.Models;

namespace Game.Features.Drop;

public interface IDropService
{
    Task<Result<Item?>> GenerateDrop(Monster monster);
}
