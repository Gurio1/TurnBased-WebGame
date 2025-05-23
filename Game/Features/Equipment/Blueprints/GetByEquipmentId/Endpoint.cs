using System.Globalization;
using FastEndpoints;
using Game.Application.SharedKernel;

namespace Game.Features.Equipment.Blueprints.GetByEquipmentId;

public sealed class Endpoint(IDispatcher dispatcher) : Endpoint<GetQuery>
{
    public override void Configure() => Get(EndpointSettings.EndpointName + "/{EquipmentId}");
    
    public override async Task HandleAsync(GetQuery req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendOkAsync(result.Value, ct);
    }
}
