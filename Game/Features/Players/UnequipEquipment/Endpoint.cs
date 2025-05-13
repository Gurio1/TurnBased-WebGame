using System.Globalization;
using FastEndpoints;
using Game.Core.SharedKernel;

namespace Game.Features.Players.UnequipEquipment;

public sealed class Endpoint : Endpoint<UnequipCommand>
{
    private readonly IDispatcher dispatcher;
    
    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public override void Configure()
    {
        Post(EndpointSettings.EndpointName + "/unequip/{EquipmentSlot}");
        Description(x => x.Accepts<UnequipCommand>());
    }
    
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
        
        await SendOkAsync(result.Value,ct);
    }
}
