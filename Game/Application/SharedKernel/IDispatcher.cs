namespace Game.Application.SharedKernel;

public interface IRequest<TResponse>;

public interface INotification;

public interface IDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
    
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default);
}
