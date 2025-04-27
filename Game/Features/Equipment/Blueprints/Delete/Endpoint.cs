using System.Globalization;
using FastEndpoints;
using Game.Core.Common;

namespace Game.Features.Equipment.Blueprints.Delete;

public sealed class Endpoint(IDispatcher dispatcher) : Endpoint<DeleteCommand>
{
    public override void Configure() => Delete(EndpointSettings.EndpointName + "/{BlueprintId}");
    
    public override async Task HandleAsync(DeleteCommand req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendNoContentAsync(ct);
    }
}
