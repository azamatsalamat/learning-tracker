using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LearningTracker.Domain.Entities;

namespace LearningTracker.Infrastructure.DataAccess.EntityConfigurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable(nameof(Profile));
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.CreationDate).IsRequired();
        
        builder.Property(p => p.Name!.FirstName).IsRequired(false).HasMaxLength(100);
        builder.Property(p => p.Name!.LastName).IsRequired(false).HasMaxLength(100);
        
        builder.Property(p => p.Address!.City).IsRequired(false).HasMaxLength(100);
        builder.Property(p => p.Address!.Country).IsRequired(false).HasMaxLength(100);
        
        builder.Property(p => p.Email).IsRequired(false).HasMaxLength(255);
        builder.Property(p => p.Phone).IsRequired(false).HasMaxLength(20);
        builder.Property(p => p.Summary).IsRequired(false).HasColumnType("text");
        builder.Property(p => p.Skills).IsRequired(false).HasColumnType("text[]");
        builder.Property(p => p.Languages).IsRequired(false).HasColumnType("text[]");
        
        builder.Property(p => p.Experiences).IsRequired(false).HasColumnType("jsonb");
        builder.Property(p => p.Educations).IsRequired(false).HasColumnType("jsonb");
        builder.Property(p => p.PersonalProjects).IsRequired(false).HasColumnType("jsonb");
        builder.Property(p => p.Certifications).IsRequired(false).HasColumnType("jsonb");
        builder.Property(p => p.Publications).IsRequired(false).HasColumnType("jsonb");
        builder.Property(p => p.Awards).IsRequired(false).HasColumnType("jsonb");
        
        builder.HasIndex(p => p.CreationDate);
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Profile>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

