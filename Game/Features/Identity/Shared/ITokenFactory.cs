namespace Game.Features.Identity.Shared;

public interface ITokenFactory
{
    public string CreateToken(User user,IConfiguration configuration);
}
