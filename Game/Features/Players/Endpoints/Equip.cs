using FastEndpoints;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Features.Players.Contracts;

namespace Game.Features.Players.Endpoints;


//TODO : If player is in battle he cannot equip or sell items.Only use consumables
public class Equip : Endpoint<EquipRequest>
{
    private readonly PlayersService _playersService;

    public Equip(PlayersService playersService)
    {
        _playersService = playersService;
    }
    
    public override void Configure()
    {
        Post("/players/equip/{ItemId}");
        Description(x => x.Accepts<EquipRequest>());
    }

    public override async Task HandleAsync(EquipRequest req, CancellationToken ct)
    {
        var player = await _playersService.GetByIdWithAbilities(req.PlayerId);

        var item = player.Inventory.FirstOrDefault(i => i.Id == req.ItemId);

        if (item is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!item.CanInteract(ItemInteractions.Equip))
        {
            ThrowError($"Can not equip item {item.Name}");
        }

        if (item is EquipmentBase equipment)
        {
            player.Equip(equipment);
        }
        Logger.LogInformation($"Equiped item - {item.Id} - {item.Name}");

        await _playersService.UpdateAsync(player);

        await SendOkAsync(player.ToViewModel(), ct);

    }
}