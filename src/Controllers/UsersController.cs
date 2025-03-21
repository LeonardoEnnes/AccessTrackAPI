using AccessTrackAPI.Data;
using AccessTrackAPI.Extensions;
using AccessTrackAPI.Models;
using AccessTrackAPI.Services;
using AccessTrackAPI.ViewModels;
using AccessTrackAPI.ViewModels.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace AccessTrackAPI.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    //Note: Shouldn't it be an admin to let a user be created? (think abt it)
    [HttpPost("v1/Users/")]
    public async Task<ActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] AccessControlContext context)
    {

        if (model == null)
            return BadRequest("User data is required.");
        
        var passwordHash = PasswordHasher.Hash(model.Password);
        
        var newUser = new Users
        {
            Name = model.Name,
            Email = model.Email,
            PasswordHash = passwordHash,
            Role = "user"
        };

        try
        {
            // Add the user to the database
            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                user = newUser.Email,
                password = model.Password, // Return the user-provided password (just for now)
                message = "Account created successfully."
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

    // @desc: Login is responsible to enter in the system and see all the info, entry and exit logs from the user itself. 
    [HttpPost("v1/Users/login")]
    public async Task<ActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] AccessControlContext context,
        [FromServices] TokenService tokenService)
    {
        if(!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>("00x00 - Internal Server Error."));
        
        var user = await context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == model.Email);
        
        if(user == null)
            return BadRequest(new ResultViewModel<string>("Username or password is incorrect."));
        
        if(!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return BadRequest(new ResultViewModel<string>("Username or password is incorrect."));

        try
        {
            // Generate the user Claims
            var claims = user.GetClaims();
            
            var token = tokenService.GenerateToken(claims);
            // here show a list of entry/exit logs 
            return Ok(new ResultViewModel<string>(token, null)); // Null is required 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(400, new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
}