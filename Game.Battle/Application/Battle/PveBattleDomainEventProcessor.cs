using Game.Battle.Application.Battle.EventHandlers;
using Game.Battle.Core.Battle.PVE.Events;
using Game.SharedKernel.Domain;

namespace Game.Battle.Application.Battle;

public sealed class PveBattleDomainEventProcessor(
    PveBattleVictoryHandler battleWonHandler,
    PveBattleDefeatHandler battleLostHandler) : IPveBattleDomainEventProcessor
{
    public async Task Process(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            switch (domainEvent)
            {
                case PveBattleWon won:
                    await battleWonHandler.Handle(won, ct);
                    break;
                case PveBattleLost lost:
                    await battleLostHandler.Handle(lost, ct);
                    break;
            }
        }
    }
}
