using Microsoft.AspNetCore.Mvc;

namespace AccessTrackAPI.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Get()
    {
        return Ok(new { message = "API is Working" });
    }
}