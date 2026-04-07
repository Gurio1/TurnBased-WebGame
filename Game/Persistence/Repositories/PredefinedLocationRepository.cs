using Game.Core.Location;
using Game.SharedKernel.Results;

namespace Game.Persistence.Repositories;

public sealed class PredefinedLocationRepository : ILocationRepository
{
    private static readonly Dictionary<string, Func<Location>> Locations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["newcomersvillage"] = static () => new NewcomersVillage()
    };

    public Task<Result<Location>> GetByName(string locationName, CancellationToken ct = default)
    {
        string key = NormalizeLocationName(locationName);

        return Task.FromResult(
            Locations.TryGetValue(key, out var createLocation)
                ? Result<Location>.Success(createLocation())
                : Result<Location>.NotFound($"Location '{locationName}' was not found."));
    }

    private static string NormalizeLocationName(string locationName) =>
        new(locationName.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
}
