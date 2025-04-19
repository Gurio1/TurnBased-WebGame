using System.Globalization;
using FastEndpoints;
using Game.Core;
using Game.Features.Players.Contracts;

namespace Game.Features.Players.Endpoints;

public sealed class Unequip : Endpoint<UnequipRequest>
{
    private readonly IPlayersMongoRepository playersMongoRepository;

    public Unequip(IPlayersMongoRepository playersMongoRepository) => 
        this.playersMongoRepository = playersMongoRepository;
    
    public override void Configure()
    {
        Post("/players/unequip/{EquipmentSlot}");
        Description(x => x.Accepts<UnequipRequest>());
    }

    public override async Task HandleAsync(UnequipRequest req, CancellationToken ct)
    {
        var playerResult = await playersMongoRepository.GetByIdWithAbilities(req.PlayerId);

        if (playerResult.IsFailure)
        {
            await SendAsync(playerResult.Error.Description, Convert.ToInt32(playerResult.Error.Code,CultureInfo.InvariantCulture), ct);
            return;
        }

        var player = playerResult.Value;
        
        if (!player.Unequip(req.EquipmentSlot)) 
            ThrowError(r => r.EquipmentSlot,$"Player doesnt have equipment item on slot : {req.EquipmentSlot}");
        
        Logger.LogInformation("Unequipped item - {Slot}",req.EquipmentSlot);

        await playersMongoRepository.UpdateAsync(player);

        await SendOkAsync(player.ToViewModel(), ct);

    }
}
