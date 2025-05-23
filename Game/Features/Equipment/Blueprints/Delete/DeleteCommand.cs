using Game.Application.SharedKernel;

namespace Game.Features.Equipment.Blueprints.Delete;

public sealed record DeleteCommand(string BlueprintId) : IRequest<ResultWithoutValue>;
