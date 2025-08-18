using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LearningTracker.Domain.Entities;

namespace LearningTracker.Infrastructure.DataAccess.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(u => u.CreationDate)
            .HasColumnName("creation_date")
            .IsRequired();

        builder.Property(u => u.Login)
            .HasColumnName("login")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasColumnName("password")
            .HasMaxLength(255)
            .IsRequired();

        // Create unique index on login
        builder.HasIndex(u => u.Login)
            .IsUnique();

        // Create index on creation date for performance
        builder.HasIndex(u => u.CreationDate);
    }
}

