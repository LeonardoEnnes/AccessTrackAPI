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
    
    // Método de extensão para Admins
    public static IEnumerable<Claim> GetClaims(this Admins admin)
    {
        var result = new List<Claim>
        {
            new(ClaimTypes.Name, admin.Email), 
            new(ClaimTypes.Role, admin.Role),
            new("IsRoot", admin.IsRoot.ToString())
        };

        return result;
    }
}