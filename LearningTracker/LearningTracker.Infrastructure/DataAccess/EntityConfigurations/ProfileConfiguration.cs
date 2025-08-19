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
        
        builder.Property(p => p.Name!.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(100);
        builder.Property(p => p.Name!.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(100);
        
        builder.Property(p => p.Address!.City)
            .HasColumnName("City")
            .HasMaxLength(100);
        builder.Property(p => p.Address!.Country)
            .HasColumnName("Country")
            .HasMaxLength(100);
        
        builder.Property(p => p.Email).HasMaxLength(255);
        builder.Property(p => p.Phone).HasMaxLength(20);
        builder.Property(p => p.Summary).HasColumnType("text");
        builder.Property(p => p.Skills).HasColumnType("text[]");
        builder.Property(p => p.Languages).HasColumnType("text[]");
        
        builder.Property(p => p.Experiences).HasColumnType("jsonb");
        builder.Property(p => p.Educations).HasColumnType("jsonb");
        builder.Property(p => p.PersonalProjects).HasColumnType("jsonb");
        builder.Property(p => p.Certifications).HasColumnType("jsonb");
        builder.Property(p => p.Publications).HasColumnType("jsonb");
        builder.Property(p => p.Awards).HasColumnType("jsonb");
        
        builder.HasIndex(p => p.CreationDate);
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Profile>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

