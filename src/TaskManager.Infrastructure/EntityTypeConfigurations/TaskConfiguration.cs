using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Infrastructure.EntityTypeConfigurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        // Primary key
        builder.HasKey(t => t.Id);

        // Title is required with a maximum length of 200 characters
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Description is optional with a maximum length of 1000 characters
        builder.Property(t => t.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        // DueDate is optional
        builder.Property(t => t.DueDate)
            .IsRequired(false);

        // Status is required and stored as an integer (default is Pending)
        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        // Priority is required and stored as an integer (default is Medium)
        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<string>();

        // CreatedAt and UpdatedAt are automatically set
        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("DATEADD(HOUR, 3, GETUTCDATE())");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("DATEADD(HOUR, 3, GETUTCDATE())")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

        // Foreign key relationship with User (Many Tasks to One User)
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // When a user is deleted, their tasks are deleted as well
    }
}