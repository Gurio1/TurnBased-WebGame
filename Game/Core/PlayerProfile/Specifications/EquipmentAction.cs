using Game.Core.PlayerProfile.Aggregates;
using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core.PlayerProfile.Specifications;

public sealed class EquipmentAction : ISpecification<GamePlayer>
{
    public ResultWithoutValue IsSatisfiedBy(GamePlayer candidate) => ResultWithoutValue.Success();
}
