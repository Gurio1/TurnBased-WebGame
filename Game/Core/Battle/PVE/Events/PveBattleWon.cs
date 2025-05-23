using Game.Application.SharedKernel;
using Game.Core.Models;

namespace Game.Core.Battle.PVE.Events;

public record PveBattleWon(CombatPlayer CombatPlayer, Monster Monster) : INotification;
