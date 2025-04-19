using Game.Core.Common;

namespace Game.Features.Equipment.Blueprints.Delete;

public sealed record Command(string BlueprintId) : IRequest<ResultWithoutValue>;
