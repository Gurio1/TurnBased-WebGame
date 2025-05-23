using Game.Application.SharedKernel;

namespace Game.Features.Identity.CreateUser;

public struct CreatePlayerCommand : IRequest<Result<string>>;
