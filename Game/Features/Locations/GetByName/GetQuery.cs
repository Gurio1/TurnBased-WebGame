using Game.Core.Location;
using Game.SharedKernel;

namespace Game.Features.Locations.GetByName;

public record GetQuery(string LocationName) : IRequest<Result<Location>>;
