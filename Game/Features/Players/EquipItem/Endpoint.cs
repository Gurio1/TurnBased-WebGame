using System.Globalization;
using FastEndpoints;
using Game.Application.SharedKernel;
using Game.Utilities;

namespace Game.Features.Players.EquipItem;

//TODO : If player is in battle he cannot equip or sell items.Only use consumables

public sealed class Endpoint : Endpoint<EquipCommand>
{
    private readonly IDispatcher dispatcher;
    
    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public override void Configure()
    {
        Post(EndpointSettings.DefaultName + "/equip/{ItemId}");
        Description(x => x.Accepts<EquipCommand>());
        Options(o => o.WithName(EndpointNames.EquipItemEndpoint));
    }
    
    public override async Task HandleAsync(EquipCommand req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description,
                Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendOkAsync(result.Value, ct);
    }
}
