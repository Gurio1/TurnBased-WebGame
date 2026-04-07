using Game.SharedKernel.Domain;

namespace Game.Battle.Application.Battle;

public interface IPveBattleDomainEventProcessor
{
    Task Process(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default);
}
