using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using TaskManager.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adds controller support and configures JSON to serialize enums as strings.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Adds API explorer for Swagger documentation.
builder.Services.AddEndpointsApiExplorer();

// Configures Swagger with JWT Bearer authentication.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Defines security requirements for using the Bearer token.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }});
});

// Adds custom dependencies from your configuration.
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// Migrates the database if the application is running in production.
if (app.Environment.IsProduction())
{
    app.MigrateDatabase();
}

// Configures the application to use Swagger and Swagger UI.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API V1");
    c.RoutePrefix = string.Empty;
});

// Enforces HTTPS.
app.UseHttpsRedirection();

// Adds authentication and authorization middleware.
app.UseAuthentication();
app.UseAuthorization();

// Maps controllers for handling API requests.
app.MapControllers();

// Runs the application.
app.Run();
