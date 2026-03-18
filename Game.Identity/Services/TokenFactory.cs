using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Game.Identity.Core;
using Microsoft.IdentityModel.Tokens;

namespace Game.Identity.Services;

public class TokenFactory : ITokenFactory
{
    public string CreateToken(User user, IConfiguration configuration)
    {
        string jwtSecret = configuration["Auth:JwtSecret"]!;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("UserId", user.Id),
            new Claim("PlayerId", user.PlayerId),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
