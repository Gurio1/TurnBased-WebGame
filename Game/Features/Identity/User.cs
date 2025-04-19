using Microsoft.AspNetCore.Identity;

namespace Game.Features.Identity;

public class User : IdentityUser
{
    public string PlayerId { get; set; }
}
