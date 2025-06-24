using FastEndpoints;
using Game.Application.SharedKernel;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;

namespace Game.Features.Players.GetById;

public sealed class GetQuery : IRequest<Result<GamePlayer>>
{
    [FromClaim] public required string PlayerId { get; set; }
}
