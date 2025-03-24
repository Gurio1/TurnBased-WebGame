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
    private readonly PlayersService _playersService;

    public Unequip(PlayersService playersService)
    {
        _playersService = playersService;
    }
    
    public override void Configure()
    {
        Post("/players/unequip/{EquipmentSlot}");
        Description(x => x.Accepts<UnequipRequest>());
    }

    public override async Task HandleAsync(UnequipRequest req, CancellationToken ct)
    {
        var player = await _playersService.GetByIdWithAbilities(req.PlayerId);

        if (!player.Unequip(req.EquipmentSlot)) 
            ThrowError(r => r.EquipmentSlot,$"Player doesnt have equipment item on slot : {req.EquipmentSlot}");
        
        Logger.LogInformation("Unequipped item - {Slot}",req.EquipmentSlot);

        await _playersService.UpdateAsync(player);

        await SendOkAsync(player.ToViewModel(), ct);

    }
}