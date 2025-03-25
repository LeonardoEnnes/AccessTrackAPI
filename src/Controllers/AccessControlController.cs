using AccessTrackAPI.Data;
using AccessTrackAPI.Models;
using AccessTrackAPI.ViewModels;
using AccessTrackAPI.ViewModels.Accounts;
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
            return BadRequest(new ResultViewModel<string>("Invalid request data."));
        
        var users = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == model.Email);
        
        if (users == null || !PasswordHasher.Verify(users.PasswordHash, model.Password))
            return BadRequest(new ResultViewModel<string>("Invalid credentials.")); 

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
                    message = "Access granted successfully!",
                    User = users.Name,
                    entryTime = entryLog.EntryTime,
                }));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("00x10 - Database Update Error."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("00x00 - internal server error."));
        }
    }
}