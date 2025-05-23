namespace Game.Application.SharedKernel;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

// We do this to avoid explicitly defining the generic type of the RequestHandler in Dispatcher<>.cs
public abstract class RequestHandlerBase
{
    public abstract Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

//We do this to avoid explicitly defining the generic type of the DispatchAsync<TRequest, TResponse> method in Dispatcher.cs
public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
{
    public abstract Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

public class RequestHandlerWrapper<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((IRequest<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(false);
    
    public override Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .Handle((TRequest)request, cancellationToken);
}
