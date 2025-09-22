using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using server_mental_maps.routes;

namespace server_mental_maps.Service;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateAccessToken(TokenGeneration userInfo)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name, userInfo.username),
            new(JwtRegisteredClaimNames.Email, userInfo.email),
            new(ClaimTypes.Expiration, DateTime.UtcNow.AddMinutes(1).ToString())
        };

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(1),
            Issuer = _configuration["Jwt:Issuer"],
            //Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var Token = tokenHandler.WriteToken(token);

        return Token;
    }

    public string CreateRefreshToken(TokenGeneration userInfo)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name, userInfo.username),
            new(JwtRegisteredClaimNames.Email, userInfo.email),
            new("token_type", "refresh_token"),
            new(ClaimTypes.Expiration, DateTime.UtcNow.AddDays(7).ToString())
        };

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            //Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var Token = tokenHandler.WriteToken(token);

        return Token;
    }

    public string RenewToken(string token)
    {
        throw new NotImplementedException();
    }
}