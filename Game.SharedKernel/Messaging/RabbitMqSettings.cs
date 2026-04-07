namespace Game.SharedKernel.Messaging;

public sealed class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";
    public string VirtualHost { get; set; } = "/";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";

    public string BattleSettlementRequestQueue { get; set; } = "battle.settlement.requests";
    public string PlayerCreateRequestQueue { get; set; } = "game.players.create.requests";
    public string PlayerDeleteRequestQueue { get; set; } = "game.players.delete.requests";
    public string BattleStartSnapshotRequestQueue { get; set; } = "game.battle.start-snapshot.requests";
}
