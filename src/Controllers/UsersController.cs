using AccessTrackAPI.Data;
using AccessTrackAPI.Models;
using AccessTrackAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

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

        var newUser = new Users
        {
            Name = user.Name,
            Email = user.Email,
            Role = "user"
        };
        
        var password = PasswordGenerator.Generate(25);
        newUser.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            // Add the user to the database
            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                user = newUser // user.Email, password (just this)
            }));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("01x00 - Email already in use."));
        }
        catch
        {
            return StatusCode(400, new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
    
}