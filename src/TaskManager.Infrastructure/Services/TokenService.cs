using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    // Constructor that injects the configuration settings (like JWT secret, issuer, etc.).
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Generates a JWT token for the given user.
    public string CreateToken(User user)
    {
        // Defines claims based on user information (ID, Username, Email).
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Gets the JWT secret key from the configuration and creates signing credentials.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                            ?? throw new InvalidOperationException("Jwt:Key is missing in appsettings.json")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Creates the JWT token with claims, expiration, and signing credentials.
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60), // Token valid for 60 minutes
            signingCredentials: credentials);

        // Returns the token string.
        return new JwtSecurityTokenHandler().WriteToken(token); 
    }
}
