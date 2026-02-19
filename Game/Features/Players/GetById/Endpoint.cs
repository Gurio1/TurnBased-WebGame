using System.Globalization;
using FastEndpoints;
using Game.Application.SharedKernel;
using Game.Contracts;
using Game.Utilities;
using Game.Utilities.Extensions;

namespace Game.Features.Players.GetById;

public sealed class Endpoint : Endpoint<GetQuery>
{
    private readonly IDispatcher dispatcher;
    private readonly UrlBuilder urlBuilder;
    
    public Endpoint(IDispatcher dispatcher, UrlBuilder urlBuilder)
    {
        this.dispatcher = dispatcher;
        this.urlBuilder = urlBuilder;
    }
    
    public override void Configure() => Get(EndpointSettings.DefaultName);
    
    public override async Task HandleAsync(GetQuery req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description,
                Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendOkAsync(result.Value.ToViewModel(urlBuilder), ct);
    }
}
