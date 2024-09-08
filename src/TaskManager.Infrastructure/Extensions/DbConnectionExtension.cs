using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Infrastructure.Extensions;

public static class DbConnectionExtension
{
    // Extension method to add the Entity Framework database connection to the service collection.
    public static void AddDbEfConnection(this IServiceCollection service, IConfiguration configuration)
    { 
        // Configures the ApplicationDbContext to use SQL Server with the connection string from configuration.
        service.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}