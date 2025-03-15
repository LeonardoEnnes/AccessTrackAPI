using AccessTrackAPI.Data;
using AccessTrackAPI.Models;
using AccessTrackAPI.Services;
using AccessTrackAPI.ViewModels;
using AccessTrackAPI.ViewModels.Accounts;
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
        [FromBody] RegisterViewModel model,
        [FromServices] AccessControlContext context)
    {

        if (model == null)
            return BadRequest("User data is required.");

        var newUser = new Users
        {
            Name = model.Name,
            Email = model.Email,
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
                user = newUser.Email, password // user.Email, password (just this)
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
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null)); // passando nul para ele n entender como erro
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(400, new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
}