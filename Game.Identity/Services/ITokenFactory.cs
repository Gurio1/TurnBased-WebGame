using Game.Identity.Core;

namespace Game.Identity.Services;

public interface ITokenFactory
{
    public string CreateToken(User user, IConfiguration configuration);
}
