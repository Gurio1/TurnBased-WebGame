using FastEndpoints;
using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Features.Players.GetById;

public sealed class GetQuery : IRequest<Result<Player>>
{
    [FromClaim]
    public required string PlayerId { get; set; }
}
