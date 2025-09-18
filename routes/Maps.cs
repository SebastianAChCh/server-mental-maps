using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server_mental_maps.routes;

[Authorize]
[ApiController]
[Route("api/maps")]
public class Maps : ControllerBase
{
    [HttpPost("create")]
    public IActionResult CreateMap([FromBody] string mapData)
    {
        return Ok("Map created");
    }


}