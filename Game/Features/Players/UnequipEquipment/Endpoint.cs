using System.Globalization;
using FastEndpoints;
using Game.Core.Common;

namespace Game.Features.Players.UnequipEquipment;

public sealed class Endpoint : Endpoint<UnequipCommand>
{
    private readonly IDispatcher dispatcher;
    
    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public override void Configure() => Post(EndpointSettings.EndpointName + "/unequip/{ItemId}");
    
    public override async Task HandleAsync(UnequipCommand req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description,
                Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendNoContentAsync(ct);
    }
}
