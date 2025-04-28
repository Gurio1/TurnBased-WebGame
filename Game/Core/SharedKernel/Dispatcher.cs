using System.Collections.Concurrent;

namespace Game.Core.SharedKernel;

public class Dispatcher : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> requestHandlers = new();
    private readonly IServiceProvider serviceProvider;
    
    public Dispatcher(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;
    
    public Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var handler = (RequestHandlerWrapper<TResponse>)requestHandlers.GetOrAdd(request.GetType(),
            static requestType =>
            {
                var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, typeof(TResponse));
                object wrapper = Activator.CreateInstance(wrapperType) ??
                                 throw new InvalidOperationException(
                                     $"Could not create wrapper type for {requestType}");
                return (RequestHandlerBase)wrapper;
            });
        
        return handler.Handle(request, serviceProvider, cancellationToken);
    }
}
