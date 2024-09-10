using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Infrastructure.Extensions;

public static class DbConnectionExtensions
{
    // Extension method to add the Entity Framework database connection to the service collection.
    public static void AddDbEfConnection(this IServiceCollection service, IConfiguration configuration)
    { 
        // Configures the ApplicationDbContext to use SQL Server with the connection string from configuration.
        service.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
    
    // Extension method to migrate the database.
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        // Creates a scope to resolve the ApplicationDbContext service and migrate the database.
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate(); 
    }
}