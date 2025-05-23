using Game.Application.SharedKernel;

namespace Game.Core.Battle.PVE.Events;

public record PveBattleLost(CombatPlayer CombatPlayer) : INotification;
