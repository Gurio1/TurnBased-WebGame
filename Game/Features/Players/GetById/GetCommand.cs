using FastEndpoints;
using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Features.Players.GetById;

public sealed class GetCommand : IRequest<Result<Player>>
{
    public required string PlayerId { get; set; }
}
