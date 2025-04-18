using System.Globalization;
using FastEndpoints;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Features.Players.Contracts;

namespace Game.Features.Players.Endpoints;


//TODO : If player is in battle he cannot equip or sell items.Only use consumables
public sealed class Equip : Endpoint<EquipRequest>
{
    private readonly IPlayersMongoRepository playersMongoRepository;

    public Equip(IPlayersMongoRepository playersMongoRepository) => 
        this.playersMongoRepository = playersMongoRepository;
    
    public override void Configure()
    {
        Post("/players/equip/{ItemId}");
        Description(x => x.Accepts<EquipRequest>());
    }

    public override async Task HandleAsync(EquipRequest req, CancellationToken ct)
    {
        var playerResult = await playersMongoRepository.GetByIdWithAbilities(req.PlayerId);

        if (playerResult.IsFailure)
        {
            await SendAsync(playerResult.Error.Description,Convert.ToInt32(playerResult.Error.Code,CultureInfo.InvariantCulture), ct);
            return;
        }

        var player = playerResult.Value;

        var item = player.Inventory.Select(s => s.Item).FirstOrDefault(i => i.Id == req.ItemId);

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
        
        Logger.LogInformation("Equiped item - {ItemId} - {ItemName}", item.Id, item.Name);

        await playersMongoRepository.UpdateAsync(player);

        await SendOkAsync(player.ToViewModel(), ct);

    }
}
