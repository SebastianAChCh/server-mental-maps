using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server_mental_maps.routes;
public class MapCreateRequest
{
    public string mapData { get; set; } = string.Empty;
}
[Authorize]
[ApiController]
[Route("api/maps")]
public class Maps : ControllerBase
{
    [HttpPost("create")]
    public IActionResult CreateMap([FromBody] MapCreateRequest mapData)
    {
        Console.WriteLine("Map data received: " + mapData.mapData);
        return Ok("Map created");
    }


}