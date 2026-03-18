using Game.SharedKernel;

namespace Game.Features.Players.Create;

public record CreateCommand : IRequest<Result<string>>;
