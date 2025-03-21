using AccessTrackAPI.Data;
using AccessTrackAPI.Models;
using AccessTrackAPI.ViewModels;
using AccessTrackAPI.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace AccessTrackAPI.Controllers;

[ApiController]
public class AccessControlController : ControllerBase
{
    [HttpPost("v1/Entry/validate-entry")]
    public async Task<ActionResult> ValidateEntry(
        [FromBody] LoginViewModel model,
        [FromServices] AccessControlContext context
        )
    {
        if(!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>("00x00 - Internal Server Error."));
        
        var users = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (users == null)
            return BadRequest(new ResultViewModel<string>("username or password is incorrect."));
        
        if (!PasswordHasher.Verify(users.PasswordHash, model.Password))
            return BadRequest(new ResultViewModel<string>("username or password is incorrect."));

        try
        {
            // logging the entry
            var entryLog = new EntryExitLogs
            {
                UserId = users.Id,
                EntryTime = DateTime.UtcNow
            };
            
            // add the entry logs to the database
            await context.EntryExitLogs.AddAsync(entryLog);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<dynamic>(new
                {
                    User = users.Email,
                    entryTime = entryLog.EntryTime,
                    message = "Entry logged successfully."
                }));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("00x10 - Database Update Error."));
        }
        catch
        {
            return StatusCode(400, new ResultViewModel<string>("00x00 - internal server error."));
        }
    }
    
    // @desc: remove users from the system
    [HttpDelete("v1/Admin/DeleteUser/{id}")]
    [Authorize(Roles = "admin")] // Requires a valid token and the "admin" role
    public async Task<IActionResult> DeleteUser(
        [FromRoute] int id,
        [FromServices] AccessControlContext context)
    {
        try
        {
            // Searching the user by id
            var user = await context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return BadRequest(new ResultViewModel<string>("User not found."));
            
            // Removing the user from the system
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            
            return Ok(new ResultViewModel<string>($"User {id} deleted successfully."));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("00x10 - Database Update Error."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("00x01 - Internal server error."));
        }
    }
}