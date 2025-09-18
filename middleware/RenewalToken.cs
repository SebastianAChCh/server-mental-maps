using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using server_mental_maps.routes;
using server_mental_maps.Service;

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

        if (context.Request.Headers.TryGetValue("refreshToken", out var refreshToken))
        {
            SecurityToken validatedToken = null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    //ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "")),
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    // ValidAudience = _configuration["Jwt:Audience"],
                }, out validatedToken);

                if (validatedToken != null)
                {
                    var jwtToken = validatedToken as JwtSecurityToken;
                    var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                    var username = jwtToken?.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

                    var accessToken = _tokenService.CreateRefreshToken(new TokenGeneration { email = email ?? "", username = username ?? "" });

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync($"{{\"Bearer\": \"{accessToken}\"}}");
                    await _next(context);
                }
            }
            catch
            {
            }

        }
        else if (!context.Request.Headers.TryGetValue("refreshToken", out var refreshTokens))
        {
            throw new UnauthorizedAccessException("No authorized");

        }

        await _next(context);
    }
}