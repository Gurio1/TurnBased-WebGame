namespace Game.Application.SharedKernel;

public abstract class NotificationHandlerWrapper
{
    public abstract Task Handle(INotification notification, IServiceProvider serviceFactory,
        CancellationToken cancellationToken);
}

public class NotificationHandlerWrapperImplementation<TNotification> : NotificationHandlerWrapper
    where TNotification : INotification
{
    public override Task Handle(INotification notification, IServiceProvider serviceFactory,
        CancellationToken cancellationToken)
    {
        var handlers = serviceFactory
            .GetServices<INotificationHandler<TNotification>>();
        
        return Publish(handlers, notification, cancellationToken);
    }
    
    private static Task Publish(IEnumerable<INotificationHandler<TNotification>> handlers, INotification notification,
        CancellationToken cancellationToken)
    {
        var tasks = handlers
            .Select(handler => handler.Handle((TNotification)notification, cancellationToken))
            .ToArray();
        
        return Task.WhenAll(tasks);
    }
}
