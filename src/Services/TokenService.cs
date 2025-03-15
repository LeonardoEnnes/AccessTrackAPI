using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AccessTrackAPI.Extensions;
using AccessTrackAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AccessTrackAPI.Services;

public class TokenService
{
    public string GenerateToken(Users user)
    {
        // Cria um manipulador de tokens JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        
        // Obtém a chave secreta do JWT a partir da configuração
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

        // colocar em outro arquivo??
        // Cria uma lista de claims com base no Usuario
        var claims = user.GetClaims();

        // configura o token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims), // Define as classes
            Expires = DateTime.UtcNow.AddHours(9), // Define a expiração do token
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // define as credenciais de assinatura 

        };
        
        // Gera o Token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Retorna o token como uma string
        return tokenHandler.WriteToken(token);
    }
}