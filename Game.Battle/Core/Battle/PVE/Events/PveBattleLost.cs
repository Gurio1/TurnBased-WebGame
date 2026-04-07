using Game.Battle.Core.Models;
using Game.SharedKernel.Domain;

namespace Game.Battle.Core.Battle.PVE.Events;

public record PveBattleLost(Player Player) : IDomainEvent;
