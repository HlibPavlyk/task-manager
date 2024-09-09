using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.AutoMapper;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure.Extensions;

public static class DependencyContainerExtension
{
    // Extension method to add dependencies to the service collection.
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Adds the database connection configuration to the service collection.
        services.AddDbEfConnection(configuration);
        
        // Adds the AutoMapper service to the service collection.
        services.AddAutoMapperService();
        
        // Adds the repositories to the service collection.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Adds the services to the service collection.
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        
        // Adds the password hasher service to the service collection.
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

    }
}