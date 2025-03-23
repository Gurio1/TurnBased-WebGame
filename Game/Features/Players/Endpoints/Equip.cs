using FastEndpoints;
using Game.Core.Equipment;
using Game.Core.Models;

namespace Game.Features.Players.Endpoints;

public class Equip : Endpoint<EquipRequest>
{
    private readonly PlayersService _playersService;

    public Equip(PlayersService playersService)
    {
        _playersService = playersService;
    }
    
    public override void Configure()
    {
        Post("/players/equip/{EquipmentId}");
        Description(x => x.Accepts<EquipRequest>());
    }

    public override async Task HandleAsync(EquipRequest req, CancellationToken ct)
    {
        var player = await _playersService.CreateQuery()
            .GetById(req.PlayerId)
            .ExecuteAsync<Hero>();

        var item = player.Inventory.FirstOrDefault(i => i.Id == req.EquipmentId);

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

        await _playersService.UpdateAsync(player.Id,player);

        await SendOkAsync(player, ct);

    }
}