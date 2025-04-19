namespace Game.Core.Common;

public interface IRequest<TResponse>
{
}

public interface IDispatcher
{
    Task<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
