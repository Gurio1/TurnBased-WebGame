using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core.Location;

public interface ILocationRepository
{
    Task<Result<Location>> GetByName(string locationName, CancellationToken ct = default);
}
