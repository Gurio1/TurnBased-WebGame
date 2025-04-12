using Game.Features.Battle.Models;
using MediatR;

namespace Game.Features.Battle.PVE.Events;

public record PveBattleDataSentEvent(PveBattle Battle) : INotification;