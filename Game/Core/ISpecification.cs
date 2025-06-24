using Game.Application.SharedKernel;

namespace Game.Core;

public interface ISpecification<in T>
{
    ResultWithoutValue IsSatisfiedBy(T candidate);
}
