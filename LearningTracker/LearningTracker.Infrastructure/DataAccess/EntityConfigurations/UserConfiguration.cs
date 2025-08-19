using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LearningTracker.Domain.Entities;

namespace LearningTracker.Infrastructure.DataAccess.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).IsRequired();
        builder.Property(u => u.CreationDate).IsRequired();
        builder.Property(u => u.Login)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(u => u.Password)
            .HasMaxLength(255)
            .IsRequired();
        builder.HasIndex(u => u.Login)
            .IsUnique();
        builder.HasIndex(u => u.CreationDate);
    }
}

