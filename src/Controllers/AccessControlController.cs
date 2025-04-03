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
         if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>("Invalid request data."));
         
         try
         {
             // First, try to find it as a regular user
             var user = await context.Users
                 .AsNoTracking()
                 .FirstOrDefaultAsync(u => u.Email == model.Email);

             if (user != null && PasswordHasher.Verify(user.PasswordHash, model.Password))
             {
                 // regular user entry log
                 var entryLog = new EntryLogs
                 {
                     UserId = user.Id,
                     EntryTime = DateTime.UtcNow
                 };

                 await context.EntryExitLogs.AddAsync(entryLog);
                 await context.SaveChangesAsync();

                 return Ok(new ResultViewModel<dynamic>(new
                 {
                     message = "User access granted successfully!",
                     nme = user.Name,
                     role = user.Role,
                     entryTime = entryLog.EntryTime,
                     email = user.Email
                 }));
             }

             // If user was not found, then it's a visitor 
             var visitor = await context.Visitor
                 .AsNoTracking()
                 .FirstOrDefaultAsync(v => v.Email == model.Email);

             if (visitor != null)
             {
                 if (!string.IsNullOrEmpty(visitor.PasswordHash) && !PasswordHasher.Verify(visitor.PasswordHash, model.Password))
                    return BadRequest(new ResultViewModel<string>("Invalid credentials."));

                 // visitor log entry
                 var entryLog = new EntryLogs
                 {
                     VisitorId = visitor.Id,
                     EntryTime = DateTime.UtcNow
                 };

                 await context.EntryExitLogs.AddAsync(entryLog);
                 await context.SaveChangesAsync();

                 return Ok(new ResultViewModel<dynamic>(new
                 {
                     message = "Visitor access granted successfully!",
                     name = visitor.Name,
                     role = visitor.Role,
                     entryTime = entryLog.EntryTime,
                     email = visitor.Email
                 })); 
             }
             return BadRequest(new ResultViewModel<string>("Invalid credentials."));
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