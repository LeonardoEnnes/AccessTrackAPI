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
public class AdminController : ControllerBase
{
    [HttpPost("v1/Admin/CreateAdmin")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreationAdmin(
        [FromBody] RegisterViewModel model,
        [FromServices] AccessControlContext context)
    {
        if (model == null)
            return BadRequest(new ResultViewModel<string>("Invalid request data"));

        // verify is there's an admin with the same email in the Database
        var existingAdmin = await context
            .Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == model.Email);

        if (existingAdmin != null)
            return BadRequest(new ResultViewModel<string>("Registration failed."));

        var phoneNumberExists = await context.Users
            .AnyAsync(u => u.TelephoneNumber == model.TelephoneNumber);
        
        if (phoneNumberExists)
            return BadRequest(new ResultViewModel<string>("Telephone number already exists"));
        
        // password hash
        var passwordHash = PasswordHasher.Hash(model.Password);

        // create a new adm
        var newAdmin = new Admins
        {
            Email = model.Email,
            PasswordHash = passwordHash,
            Name = model.Name,
            TelephoneNumber = model.TelephoneNumber,
            Role = "admin" 
        };

        try
        {
            // add adm to the database
            await context.Admins.AddAsync(newAdmin);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<string>("Admin created successfully", null));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("00x10 - Database Update Error."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("00x00 - internal server error.\"d"));
        }
    }
    
    // @desc: This route is only dedicated for creating the first admin in the system
    [HttpPost("v1/Admin/CreateFirstAdmin")]
    public async Task<IActionResult> CreateFirstAdmin(
        [FromBody] RegisterViewModel model,
        [FromServices] AccessControlContext context)
    {
        if (model == null)
            return BadRequest(new ResultViewModel<string>("Admin data required."));
        
        // verify if an admin already exists in the database
        var anyAdmin = await context.
            Admins
            .AsNoTracking()
            .AnyAsync();
        
        if(anyAdmin)
            return BadRequest(new ResultViewModel<string>("Admin already exists."));
        
        var passwordHash = PasswordHasher.Hash(model.Password);

        var newAdmin = new Admins
        {
            Email = model.Email,
            PasswordHash = passwordHash,
            Name = model.Name,
            Role = "admin",
            TelephoneNumber = model.TelephoneNumber,
            IsRoot = true
        };

        try
        {
            await context.Admins.AddAsync(newAdmin);
            await context.SaveChangesAsync();
            
            return Ok(new ResultViewModel<dynamic>( new
            {
                admin = newAdmin,
                message = "Admin created successfully."
            }));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("00x10 - Database Update Error."));
        }
        catch
        {
            return BadRequest(new ResultViewModel<string>("00x00 - internal server error."));
        }
    }
    
    // @desc: Logging the admin in the system thus returning an JWT Token that is going to be used in other operations in the system
    [HttpPost("v1/Admin/LoginAdmin")]
    public async Task<IActionResult> LoginAdmin(
        [FromBody] LoginViewModel model,
        [FromServices] AccessControlContext context,
        [FromServices] TokenService tokenService)
    {
        if(!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>("Invalid request data."));
        
        // Searching the admin in the system
        var admin = await context
            .Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == model.Email);

        if (admin == null || !PasswordHasher.Verify(admin.PasswordHash, model.Password))
        {
            return BadRequest(new ResultViewModel<string>("Username or password is incorrect."));
        }
        
        try
        {
            // Generate admin claim
            var claims = admin.GetClaims();
            
            var token = tokenService.GenerateToken(claims);
            
            return Ok(new ResultViewModel<string>(token, null)); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(400, new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
    
    // @desc: Route to remove admins from the system
    [HttpDelete("v1/Admin/DeleteAdmin/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteAdmin(
        [FromRoute] int id,
        [FromServices] AccessControlContext context,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        // Get the authenticated user's email from the JWT token. 
        var authenticatedUserEmail = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(authenticatedUserEmail))
            return Unauthorized(new ResultViewModel<string>("User not authenticated."));

        // Get the IsRoot claim from token JWT
        var isRootClaim = httpContextAccessor.HttpContext.User.FindFirst("IsRoot")?.Value;
        bool isRoot = bool.TryParse(isRootClaim, out var result) && result;

        if (!isRoot)
            return Unauthorized(new ResultViewModel<string>("Only the root admin can delete other admins."));
        
        try
        {
            // searching the Admin by ID
            var admin = await context
                .Admins
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if(admin == null)
                return BadRequest(new ResultViewModel<string>($"Admin {id} not found."));
            
            // Prevent the admin root from deleting himself
            if (admin.IsRoot)
                return BadRequest(new ResultViewModel<string>("The root admin cannot be deleted."));
            
            context.Admins.Remove(admin);
            await context.SaveChangesAsync();
            
            return Ok(new ResultViewModel<string>($"Admin {id} deleted successfully.", null));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("00x10 - Database Update Error."));
        }
        catch
        {
            return BadRequest(new ResultViewModel<string>("00x00 - Internal Server Error."));
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
            return StatusCode(500, new ResultViewModel<string>("00x00 - Internal server error."));
        }
    }
    
    // @desc: update Users from the system
    [HttpPut("v1/Admin/UpdateUser/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] int id,
        [FromBody] RegisterViewModel model,
        [FromServices] AccessControlContext context)
    {
        try
        {
            // Find the user by Id
            var user = await context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return BadRequest(new ResultViewModel<string>("User not found."));
            
            // Update user properties
            user.Name = model.Name;
            user.Email = model.Email;
            user.TelephoneNumber = model.TelephoneNumber;
            
            // Hash the new password if provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = PasswordHasher.Hash(model.Password);
            }
            
            context.Users.Update(user);
            await context.SaveChangesAsync();
            
            return Ok(new ResultViewModel<dynamic>($"User {id} updated successfully."));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("00x10 - Database Update Error."));
        }
        catch
        {
            return BadRequest(new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
    
    // @desc: Get logs/info of one or many users in the system 
    [HttpGet("v1/Admin/UserLogs/{userId?}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUserLogs(
        [FromRoute] int? userId,
        [FromServices] AccessControlContext context)
    {
        try
        {
            IQueryable<EntryLogs> query = context
                .EntryExitLogs
                .AsNoTracking()
                .Include(log => log.User);

            if (userId.HasValue)
                query = query.Where(log => log.UserId == userId); // fetching logs for a specific user


            var logs = await query.Select(log => new EntryLogsDto
            {
                Id = log.Id,
                EntryTime = log.EntryTime,
                UserId = log.UserId,
                UserName = log.User.Name
            }).ToListAsync();

            if (logs == null || !logs.Any())
                return NotFound(new ResultViewModel<string>("No logs found."));
            
            return Ok(new ResultViewModel<List<EntryLogsDto>>(logs));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
}