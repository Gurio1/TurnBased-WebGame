using Game.Core.Models;
using Game.SharedKernel;

namespace Game.Core.Battle.PVE.Events;

public record PveBattleWon(CombatPlayer CombatPlayer, Monster Monster) : INotification;
