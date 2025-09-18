using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using server_mental_maps.models;

namespace server_mental_maps.routes;

[ApiController]
[Route("api/auth")]
public class AuthenticationRoute : ControllerBase
{
    private readonly IMongoDatabase _database;
    private readonly ITokenService _tokenService;
    public AuthenticationRoute( IMongoDatabase database, ITokenService tokenService)
    {
        _database = database;
        _tokenService = tokenService;
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] User userInfo)
    {
        if (String.IsNullOrEmpty(userInfo.Email) || String.IsNullOrEmpty(userInfo.Username) || String.IsNullOrEmpty(userInfo.Password)
        || String.IsNullOrEmpty(userInfo.name) || String.IsNullOrEmpty(userInfo.lastName))
        { 
            return BadRequest("Missing required fields");
        }

        await _database.GetCollection<User>("Users").InsertOneAsync(userInfo);

        var acessToken = _tokenService.CreateAccessToken(new TokenGeneration{ email = userInfo.Email, username = userInfo.Username});
        var refreshToken = _tokenService.CreateRefreshToken(new TokenGeneration{ email = userInfo.Email, username = userInfo.Username});

        return Ok(new { acessToken, refreshToken });
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] UserLogin userInfo)
    {
        var result = _database.GetCollection<User>("Users").Find(u => u.Email == userInfo.Email && u.Username == userInfo.Username).FirstOrDefault();
        
        if (result == null)
        {
            return Unauthorized("Invalid credentials");
        }

        var acessToken = _tokenService.CreateAccessToken(new TokenGeneration{ email = userInfo.Email, username = userInfo.Username});
        var refreshToken = _tokenService.CreateRefreshToken(new TokenGeneration{ email = userInfo.Email, username = userInfo.Username});

        return Ok(new { acessToken, refreshToken });
    }

}

