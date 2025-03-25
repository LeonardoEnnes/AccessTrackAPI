using System.Security.Claims;
using AccessTrackAPI.Data;
using AccessTrackAPI.Extensions;
using AccessTrackAPI.Models;
using AccessTrackAPI.Services;
using AccessTrackAPI.ViewModels;
using AccessTrackAPI.ViewModels.Accounts;
using AccessTrackAPI.ViewModels.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace AccessTrackAPI.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost("v1/Users/CreateUser")]
    public async Task<ActionResult> CreateUser(
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

    // @desc: Login is responsible to enter in the system  
    [HttpPost("v1/Users/login")]
    public async Task<ActionResult> LoginUser(
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
    
    //@desc: This route is responsible for showing the Logs and infos of the user logged
    [HttpGet("v1/Users/infos")]
    [Authorize] // only the user can access
    public async Task<ActionResult> GetUserInfo(
        [FromServices] AccessControlContext context)
    {
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value; // get email from the token
        
        if (userEmail == null)
        {
            Console.WriteLine("Email not found in token.");
            return BadRequest(new ResultViewModel<string>("User not found."));
        }

        Console.WriteLine($"Email from token: {userEmail}");
        
        try
        {
            // Searching for the users and logs in the database in system
            var user = await context
                .Users
                .AsNoTracking()
                .Include(u => u.EntryExitLogs)
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            
            if(user == null)
            {
                Console.WriteLine($"User with email {userEmail} not found in database.");
                return BadRequest(new ResultViewModel<string>("User not found."));
            }
            
            // Mapping logs to the DTO
            var logsDto = user.EntryExitLogs
                .Select(log => new EntryExitLogsDto
                {
                    Id = log.Id,
                    EntryTime = log.EntryTime,
                    ExitTime = log.ExitTime,
                    UserId = log.UserId,
                    UserName = user.Name // Username associated to the log
                })
                .ToList();
            
            // Create response object
            var userInfo = new
            {
                Email = user.Email,
                Name = user.Name,
                Logs = logsDto
            };
            
            return Ok(new ResultViewModel<object>(userInfo));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(400, new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
    
}