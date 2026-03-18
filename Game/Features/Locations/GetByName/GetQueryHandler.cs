using Game.Core.Location;
using Game.SharedKernel;

namespace Game.Features.Locations.GetByName;

public sealed class GetQueryHandler : IRequestHandler<GetQuery, Result<Location>>
{
    private static readonly Dictionary<string, Func<Location>> locations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["newcomersvillage"] = static () => new NewcomersVillage(),
    };

    public Task<Result<Location>> Handle(GetQuery request, CancellationToken cancellationToken)
    {
        string key = NormalizeLocationName(request.LocationName);

        if (!locations.TryGetValue(key, out var createLocation))
            return Task.FromResult(Result<Location>.NotFound($"Location '{request.LocationName}' was not found."));

        return Task.FromResult(Result<Location>.Success(createLocation()));
    }

    private static string NormalizeLocationName(string locationName) =>
        new(locationName.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
}
