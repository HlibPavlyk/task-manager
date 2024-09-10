using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Entities;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DbSet for the Users table in the database.
    public DbSet<User> Users { get; set; }

    // DbSet for the Tasks table in the database.
    public DbSet<Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applies configurations from separate classes within the same assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    // Override SaveChangesAsync to set UpdatedAt for modified entities
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentDateTime = DateTime.Now;
        var entries = ChangeTracker.Entries().ToList();

        // Get a list of all Modified entries that implement the IUpdateable interface
        var updatedEntries = entries.Where(e => e is { Entity: IUpdateable, State: EntityState.Modified }).ToList();

        // Set UpdatedAt for modified entities
        updatedEntries.ForEach(e =>
        {
            ((IUpdateable)e.Entity).UpdatedAt = currentDateTime;
        });

        return await base.SaveChangesAsync(cancellationToken);
    }

}