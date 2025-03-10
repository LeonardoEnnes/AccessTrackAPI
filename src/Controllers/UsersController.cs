using AccessTrackAPI.Data;
using AccessTrackAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccessTrackAPI.Controllers;

// CRUD
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost("v1/Users/")]
    public async Task<ActionResult> Post(
        [FromBody] Users user,
        [FromServices] AccessControlContext context)
    {

        if (user == null)
            return BadRequest("User data is required.");

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Post), new { id = user.Id }, user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine(e.InnerException?.Message);
            return StatusCode(500, "An error occurred while saving the user.");
        }
    }

}