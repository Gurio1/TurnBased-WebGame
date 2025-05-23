using System.Collections.Concurrent;

namespace Game.Application.SharedKernel;

public class Dispatcher : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> requestHandlers = new();
    private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> notificationHandlers = new();
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
    
    public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        var handler = notificationHandlers.GetOrAdd(notification.GetType(),
            static notificationType =>
            {
                var wrapperType = typeof(NotificationHandlerWrapperImplementation<>).MakeGenericType(notificationType);
                object wrapper = Activator.CreateInstance(wrapperType) ??
                                 throw new InvalidOperationException(
                                     $"Could not create wrapper type for {notificationType}");
                return (NotificationHandlerWrapper)wrapper;
            });
        
        return handler.Handle((INotification)notification, serviceProvider, cancellationToken);
    }
}
