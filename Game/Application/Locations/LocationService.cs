using Game.Core.Location;
using Game.Core.PlayerProfile;
using Game.SharedKernel.Results;

namespace Game.Application.Locations;

public sealed class LocationService(
    ILocationRepository locationRepository,
    IPlayerRepository playerRepository) : ILocationService
{
    public Task<Result<Location>> GetByName(string locationName, CancellationToken ct = default) =>
        locationRepository.GetByName(locationName, ct);

    public async Task<Result<Explore.ExploreResponse>> Explore(
        string playerId,
        string locationName,
        CancellationToken ct = default)
    {
        var locationResult = await locationRepository.GetByName(locationName, ct);
        if (locationResult.IsFailure)
            return locationResult.AsError<Explore.ExploreResponse>();

        var playerResult = await playerRepository.GetById(playerId, ct);
        if (playerResult.IsFailure)
            return playerResult.AsError<Explore.ExploreResponse>();

        var location = locationResult.Value;
        var player = playerResult.Value;
        var loot = location.Explore(player);

        var saveResult = await playerRepository.Save(player, ct);
        if (saveResult.IsFailure)
            return saveResult.AsError<Explore.ExploreResponse>();

        return loot is null
            ? Result<Explore.ExploreResponse>.Success(new Explore.ExploreResponse
            {
                LocationName = location.Name,
                Message = "You explored the area but found nothing this time.",
                Quantity = 0
            })
            : Result<Explore.ExploreResponse>.Success(new Explore.ExploreResponse
            {
                LocationName = location.Name,
                Message = $"You found {loot.Quantity}x {loot.Item.Name}.",
                ItemName = loot.Item.Name,
                ItemImageUrl = loot.Item.ImageUrl,
                Quantity = loot.Quantity
            });
    }
}
