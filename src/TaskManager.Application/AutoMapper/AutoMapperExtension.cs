using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Application.AutoMapper;

public static class AutoMapperExtension
{
    // Extension method to add AutoMapper to the service collection.
    public static void AddAutoMapperService(this IServiceCollection service)
    {
        // Registers AutoMapper and configures it using the AutoMapperProfile class.
        service.AddAutoMapper(cfg =>
        {
            // Adds the AutoMapper profile, which contains the mapping configurations.
            cfg.AddProfile<AutoMapperProfile>();
        });
    }
}
