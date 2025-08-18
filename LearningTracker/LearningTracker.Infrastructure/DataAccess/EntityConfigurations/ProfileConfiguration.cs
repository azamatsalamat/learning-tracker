using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LearningTracker.Domain.Entities;

namespace LearningTracker.Infrastructure.DataAccess.EntityConfigurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(p => p.CreationDate)
            .HasColumnName("creation_date")
            .IsRequired();

        // Create index on creation date for performance
        builder.HasIndex(p => p.CreationDate);

        // Configure one-to-one relationship with User
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Profile>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

