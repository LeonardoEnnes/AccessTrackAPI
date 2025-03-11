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
            var newUser = new Users
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            // Add the user to the database
            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), new { id = newUser.Id }, newUser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine(e.InnerException?.Message);
            return StatusCode(500, "An error occurred while saving the user.");
        }
    }

}