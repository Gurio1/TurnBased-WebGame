using System.Globalization;
using FastEndpoints;
using Game.Application.SharedKernel;

namespace Game.Features.Locations.GetByName;

public sealed class Endpoint : Endpoint<GetQuery>
{
    private readonly IDispatcher dispatcher;

    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;

    public override void Configure()
    {
        Get(EndpointSettings.DefaultName + "/{LocationName}");
        Description(x => x.Accepts<GetQuery>());
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetQuery req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);

        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture), ct);
            return;
        }

        await SendOkAsync(result.Value, ct);
    }
}
