using Game.Application.SharedKernel;
using Game.Core.Location;

namespace Game.Features.Locations.GetByName;

public record GetQuery(string LocationName) : IRequest<Result<Location>>;
