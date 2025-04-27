using FastEndpoints;
using Game.Core.Common;
using Game.Core.Models;

namespace Game.Features.Players.GetById;

public sealed class GetCommand : IRequest<Result<Player>>
{
    public required string PlayerId { get; set; }
}
