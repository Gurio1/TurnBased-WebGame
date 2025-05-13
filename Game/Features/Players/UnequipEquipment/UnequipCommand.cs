using FastEndpoints;
using Game.Core.SharedKernel;
using Game.Features.Players.Contracts;

namespace Game.Features.Players.UnequipEquipment;

public class UnequipCommand : IRequest<Result<PlayerViewModel>>
{
    [FromClaim] public required string PlayerId { get; init; }
    public required string EquipmentSlot { get; init; }
}
