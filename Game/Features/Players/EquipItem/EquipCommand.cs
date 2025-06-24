using FastEndpoints;
using Game.Application.SharedKernel;
using Game.Features.Players.Contracts;

namespace Game.Features.Players.EquipItem;

public sealed class EquipCommand : IRequest<Result<PlayerViewModel>>
{
    [FromClaim] public required string PlayerId { get; set; }
    
    public required string ItemId { get; set; }
}
