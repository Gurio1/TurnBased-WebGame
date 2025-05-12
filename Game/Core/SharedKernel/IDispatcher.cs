namespace Game.Core.SharedKernel;

public interface IRequest<TResponse>
{
}

public interface IDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}
