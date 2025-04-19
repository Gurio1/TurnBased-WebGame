using System.Globalization;
using FastEndpoints;
using Game.Core.Common;

namespace Game.Features.Equipment.Blueprints.GetByEquipmentId;

public sealed class Endpoint(IDispatcher dispatcher) : Endpoint<Query>
{
    public override void Configure() => Get(EndpointSettings.EndpointName + "/{EquipmentId}");
    
    public override async Task HandleAsync(Query req, CancellationToken ct)
    {
        var result = await dispatcher.Dispatch(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture), ct);
            return;
        }
        
        await SendOkAsync(result.Value, ct);
    }
}
