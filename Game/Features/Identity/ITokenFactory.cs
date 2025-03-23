namespace Game.Features.Identity;

public interface ITokenFactory
{
    public string CreateToken(User user,IConfiguration configuration);
}