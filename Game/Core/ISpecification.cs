using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core;

public interface ISpecification<in T>
{
    ResultWithoutValue IsSatisfiedBy(T candidate);
}
