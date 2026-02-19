using Game.Application.SharedKernel;
using Game.Core.Location;

namespace Game.Features.Locations.GetByName;

public sealed class GetQueryHandler : IRequestHandler<GetQuery, Location>
{
    public Task<Location> Handle(GetQuery request, CancellationToken cancellationToken) =>
        Task.FromResult<Location>(new NewcomersVillage());
}
