using FastEndpoints;
using Game.Application.SharedKernel;
using Game.Core.PlayerProfile;

namespace Game.Features.Players.GetById;

public sealed class GetQuery : IRequest<Result<Player>>
{
    [FromClaim] public required string PlayerId { get; set; }
}
