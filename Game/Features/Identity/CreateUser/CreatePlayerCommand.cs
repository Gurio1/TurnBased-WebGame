using Game.Core.Common;

namespace Game.Features.Identity.CreateUser;

public struct CreatePlayerCommand() : IRequest<Result<string>>;
