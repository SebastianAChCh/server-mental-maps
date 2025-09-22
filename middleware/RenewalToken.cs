using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using server_mental_maps.routes;

namespace serber_mental_maps.middleware;

public class RenewalToken
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    public RenewalToken(RequestDelegate next, IConfiguration configuration, ITokenService tokenService)
    {
        _next = next;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/api/auth/login") || path.Contains("/api/auth/signup"))
        {
            await _next(context);
            return;
        }
    
        if (context.Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var bearer = authorization.ToString();
            string token = bearer.Substring("Bearer ".Length).Trim();

            try
            {
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateIssuer = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "")),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    //ValidAudience = _configuration["Jwt:Audience"],
                    ValidateAudience = false,
                }, out var validatedToken);

                if (validatedToken != null)
                {
                    await _next(context);
                    return;
                }
            }
            catch
            {
            }
        }
        
        string accessToken = "";
        SecurityToken validatedTokenTemp;
        if (context.Request.Headers.TryGetValue("refreshtoken", out var refreshtoken))
        {

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(refreshtoken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "")),
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    // ValidAudience = _configuration["Jwt:Audience"],
                }, out validatedTokenTemp);

                if (validatedTokenTemp != null)
                {
                    var jwtToken = validatedTokenTemp as JwtSecurityToken;
                    var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                    var username = jwtToken?.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

                    accessToken = _tokenService.CreateAccessToken(new TokenGeneration { email = email ?? "", username = username ?? "" });

                    context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
                    context.Response.Headers.Append("X-New-Access-Token", accessToken);

                }
            }
            catch
            {
                throw new UnauthorizedAccessException();
            }

        }
        else
        {
            await _next(context);
            return;
        }

        await _next(context);

    }
}