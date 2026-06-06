using BackendAwSmartstay.API.IAM.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace BackendAwSmartstay.API.IAM.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        // IAM Context Configuration
        builder.Entity<User>().ToTable("users"); // Explicit table name
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Entity<User>().Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
        
        // --- NEW: ROLE COLUMN ---
        builder.Entity<User>().Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("guest");
    }
}