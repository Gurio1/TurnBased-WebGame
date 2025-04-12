using Game.Core.Models;
using Game.Features.Battle.Models;
using MediatR;

namespace Game.Features.Battle.PVE.Events;

public record MonsterDefeatedEvent(Monster Monster,CombatPlayer CombatPlayer) : INotification;