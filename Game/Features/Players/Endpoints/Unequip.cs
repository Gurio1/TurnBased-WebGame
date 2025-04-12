using FastEndpoints;
using Game.Core.Equipment;
using Game.Core.Equipment.Boots;
using Game.Core.Equipment.Chests;
using Game.Core.Equipment.Heads;
using Game.Core.Equipment.Weapons;
using Game.Features.Attributes;
using Game.Features.Players.Contracts;

namespace Game.Features.Players.Endpoints;

public class Unequip : Endpoint<UnequipRequest>
{
    private readonly IPlayersMongoRepository _playersMongoRepository;

    public Unequip(IPlayersMongoRepository playersMongoRepository)
    {
        _playersMongoRepository = playersMongoRepository;
    }
    
    public override void Configure()
    {
        Post("/players/unequip/{EquipmentSlot}");
        Description(x => x.Accepts<UnequipRequest>());
    }

    public override async Task HandleAsync(UnequipRequest req, CancellationToken ct)
    {
        var playerResult = await _playersMongoRepository.GetByIdWithAbilities(req.PlayerId);

        if (playerResult.IsFailure)
        {
            await SendAsync(playerResult.Error.Description, int.Parse(playerResult.Error.Code), ct);
            return;
        }

        var player = playerResult.Value;
        
        if (!player.Unequip(req.EquipmentSlot)) 
            ThrowError(r => r.EquipmentSlot,$"Player doesnt have equipment item on slot : {req.EquipmentSlot}");
        
        Logger.LogInformation("Unequipped item - {Slot}",req.EquipmentSlot);

        await _playersMongoRepository.UpdateAsync(player);

        await SendOkAsync(player.ToViewModel(), ct);

    }
}