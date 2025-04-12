using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Identity.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirstValue("PlayerId")!;
    }
}