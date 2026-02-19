using Microsoft.AspNetCore.Identity;

namespace Game.Core.Models;

public class User : IdentityUser
{
    public string PlayerId { get; set; }
}
