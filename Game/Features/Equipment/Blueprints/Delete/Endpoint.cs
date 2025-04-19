using System.Globalization;
using FastEndpoints;
using Game.Core.Common;

namespace Game.Features.Equipment.Blueprints.Delete;

public sealed class Endpoint(IDispatcher dispatcher) : Endpoint<Command>
{
    public override void Configure() => Delete(EndpointSettings.EndpointName + "/{BlueprintId}");
    
    public override async Task HandleAsync(Command req, CancellationToken ct)
    {
        var result = await dispatcher.Dispatch(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture), ct);
            return;
        }
        
        await SendNoContentAsync(ct);
    }
}
