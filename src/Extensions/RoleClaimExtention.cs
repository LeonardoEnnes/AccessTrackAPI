using System.Security.Claims;
using AccessTrackAPI.Models;

namespace AccessTrackAPI.Extensions;

public static class RoleClaimExtention
{
    public static IEnumerable<Claim> GetClaims(this Users user)
    {
        var result = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email) // esse item vira User.Identity.Name
        };
        
        result.AddRange(
            user.Role.Select(role => new Claim(ClaimTypes.Role, role.ToString()))
        );
        
        return result;
    }
}