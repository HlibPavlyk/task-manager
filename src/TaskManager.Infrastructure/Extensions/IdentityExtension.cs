using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace TaskManager.Infrastructure.Extensions;

public static class IdentityExtension
{
    // Extension method to add JWT authentication to the service collection.
    public static void AddJwtAuthentication(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Disable HTTPS requirement (useful for development).
                options.RequireHttpsMetadata = false;

                // Save the token in the request context.
                options.SaveToken = true;

                // Set token validation parameters.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    AuthenticationType = "Jwt", // Specify the authentication type as JWT.

                    // Validate the JWT issuer.
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],

                    // Validate the JWT audience.
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],

                    // Validate the token's lifetime.
                    ValidateLifetime = true,

                    // Define the signing key to validate the token's signature.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),

                    // Ensure the signing key is valid.
                    ValidateIssuerSigningKey = true,
                };
            });
    }
}
