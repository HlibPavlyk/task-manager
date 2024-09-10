using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.EntityTypeConfigurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary key
        builder.HasKey(u => u.Id);

        // Username is required and must be unique
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        // Email is required and must be unique
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(70);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // PasswordHash is required
        builder.Property(u => u.PasswordHash)
            .IsRequired();

        // CreatedAt is automatically set
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("GETDATE()");

        // One-to-Many relationship with Task (each User can have many Tasks)
        builder.HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade); // When user is deleted, tasks should be deleted as well
    }
}