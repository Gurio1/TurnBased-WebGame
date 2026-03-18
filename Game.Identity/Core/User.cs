using Microsoft.AspNetCore.Identity;

namespace Game.Identity.Core;

public class User : IdentityUser
{
    public string PlayerId { get; set; } = string.Empty;
}
