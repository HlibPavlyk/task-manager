using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Infrastructure.Extensions;

public static class DependencyContainerExtension
{
    // Extension method to add dependencies to the service collection.
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Adds the database connection configuration to the service collection.
        services.AddDbEfConnection(configuration);
    }
}