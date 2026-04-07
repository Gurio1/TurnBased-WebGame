using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Game.Battle.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User.FindFirstValue("PlayerId")!;
}
