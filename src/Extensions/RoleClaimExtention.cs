using System.Security.Claims;
using AccessTrackAPI.Models;

namespace AccessTrackAPI.Extensions;

public static class RoleClaimExtention
{
    public static IEnumerable<Claim> GetClaims(this Users user)
    {
        var result = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email), 
            new(ClaimTypes.Role, user.Role)
        };
        
        return result;
    }
}