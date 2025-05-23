using System.Globalization;
using FastEndpoints;
using Game.Application.SharedKernel;

namespace Game.Features.Monsters.Delete;

public sealed class Endpoint : Endpoint<DeleteCommand>
{
    private readonly IDispatcher dispatcher;
    
    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public override void Configure() =>
        Delete(EndpointSettings.EndpointName + "/{MonsterName}");
    
    public override async Task HandleAsync(DeleteCommand req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description,
                Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendOkAsync(ct);
    }
}
