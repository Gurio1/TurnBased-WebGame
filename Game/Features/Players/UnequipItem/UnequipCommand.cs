using Game.Contracts;
using Game.SharedKernel;

namespace Game.Features.Players.UnequipItem;

public class UnequipCommand : IRequest<Result<PlayerViewModel>>
{
    public required string PlayerId { get; init; }
    public required string EquipmentSlot { get; init; }
}
