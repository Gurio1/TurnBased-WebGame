using Game.Application.SharedKernel;
using Game.Core.PlayerProfile.Aggregates;

namespace Game.Core.PlayerProfile.Specifications;

public sealed class EquipmentAction : ISpecification<GamePlayer>
{
    public ResultWithoutValue IsSatisfiedBy(GamePlayer candidate) => 
        candidate.InBattle()
            ? ResultWithoutValue.Invalid("Player can not equip item during battle")
            : ResultWithoutValue.Success();
}
