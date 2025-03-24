using FastEndpoints.Security;

namespace Game.Features.Identity;

public class TokenFactory : ITokenFactory
{
    public string CreateToken(User user,IConfiguration configuration)
    {
        var jwtSecret = configuration["Auth:JwtSecret"]!;
        
        var token = JwtBearer.CreateToken(opt =>
        {
            opt.SigningKey = jwtSecret;
            opt.User["UserId"] = user.Id;
            opt.User["PlayerId"] = user.PlayerId.ToString();
        });

        return token;
    }
}