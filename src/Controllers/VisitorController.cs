using System.Security.Claims;
using AccessTrackAPI.Data;
using AccessTrackAPI.Models;
using AccessTrackAPI.ViewModels;
using AccessTrackAPI.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessTrackAPI.Controllers;

// criar um VisitorRegisterViewModels/ CreateVisitorViewModel para alocar a regra de neg do visitante, não interferindo com as outras classes 

public class VisitorController : ControllerBase
{
    // @desc: Route to create visitors
    [HttpPost("v1/visitors/CreateVisitor")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> CreateVisitor(
        [FromBody] RegisterVisitorViewModel model,
        [FromServices] AccessControlContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var existingVisitor = await context.Visitor
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (existingVisitor != null)
                return BadRequest("email already registered");
            
            // getting the admin email from the claim type
            var adminEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if(string.IsNullOrEmpty(adminEmail))
                return BadRequest(new ResultViewModel<string>("Unable to find admin email from token"));
            
            // creating the visitor
            var visitor = new Visitor
            {
                Name = model.Name,
                Email = model.Email,
                TelephoneNumber = model.TelephoneNumber,
                CreatedByAdmin = adminEmail, // Assuming admin ID is stored in the token
                Purpose = model.Purpose,
                Role = "visitor"
            };
            
            await context.Visitor.AddAsync(visitor);
            await context.SaveChangesAsync();
            
            return Ok(new ResultViewModel<dynamic>(
                new
                {
                    message = "Visitor created successfully",
                    Id = visitor.Id,
                }, null
                ));
            
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("01x00 - Email already in use."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
    
    // @desc: Route to remove visitors from the system 
    [HttpDelete("v1/visitors/DeleteVisitor/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteVisitor(
        [FromRoute] int id,
        [FromServices] AccessControlContext context)
    {
        try
        {
            // Searching the visitor
            var visitor = await context
                .Visitor
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);
            
            if(visitor == null)
                return BadRequest(new ResultViewModel<string>("Visitor not found"));
            
            // Removing the visitor
            context.Visitor.Remove(visitor);
            await context.SaveChangesAsync();
            
            return Ok(new ResultViewModel<dynamic>($"Visitor {id} deleted successfully"));
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            return StatusCode(400, new ResultViewModel<string>("00x10 - Database Update Error."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("00x00 - Internal Server Error."));
        }
    }
}