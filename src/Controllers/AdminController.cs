using AccessTrackAPI.Data;
using AccessTrackAPI.Models;
using AccessTrackAPI.ViewModels;
using AccessTrackAPI.ViewModels.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace AccessTrackAPI.Controllers;

[ApiController]
public class AdminController : ControllerBase
{

    [HttpPost("v1/Admin/CreateAdmin")]
    public async Task<IActionResult> CreationAdmin(
        [FromBody] RegisterViewModel model,
        [FromServices] AccessControlContext context)
    {
        if (model == null)
            return BadRequest(new ResultViewModel<string>("Admin data required."));

        // verify is there's an admin with the same email in the Database
        var existingAdmin = await context.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == model.Email);

        if (existingAdmin != null)
            return BadRequest(new ResultViewModel<string>("Email already exists."));

        // password hash
        var passwordHash = PasswordHasher.Hash(model.Password);

        // create a new adm
        var newAdmin = new Admins
        {
            Email = model.Email,
            PasswordHash = passwordHash,
            Name = model.Name,
            Role = "admin" // role set as adm (for now)
        };

        try
        {
            // add adm to the db
            await context.Admins.AddAsync(newAdmin);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<dynamic>(new
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
}