using Game.SharedKernel;

namespace Game.Features.Players.Delete;

public sealed record DeleteCommand(string PlayerId) : IRequest<ResultWithoutValue>;
