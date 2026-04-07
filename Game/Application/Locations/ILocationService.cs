using Game.Core.Location;
using Game.SharedKernel.Results;

namespace Game.Application.Locations;

public interface ILocationService
{
    Task<Result<Location>> GetByName(string locationName, CancellationToken ct = default);
    Task<Result<Explore.ExploreResponse>> Explore(string playerId, string locationName, CancellationToken ct = default);
}
