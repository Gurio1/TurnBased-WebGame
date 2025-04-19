using FastEndpoints.Security;

namespace Game.Features.Identity.Shared;

public class TokenFactory : ITokenFactory
{
    public string CreateToken(User user, IConfiguration configuration)
    {
        string jwtSecret = configuration["Auth:JwtSecret"]!;
        
        string token = JwtBearer.CreateToken(opt =>
        {
            opt.SigningKey = jwtSecret;
            opt.User["UserId"] = user.Id;
            opt.User["PlayerId"] = user.PlayerId;
        });
        
        return token;
    }
}
