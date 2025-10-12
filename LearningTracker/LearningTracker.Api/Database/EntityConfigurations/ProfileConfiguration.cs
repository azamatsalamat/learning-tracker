using LearningTracker.Features.Profiles.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningTracker.Database.EntityConfigurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable(nameof(Profile));
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.CreationDate).IsRequired();
        builder.Property(p => p.Email);
        builder.Property(p => p.Phone)
            .HasMaxLength(50);
        builder.ComplexProperty(p => p.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.FirstName);
            nameBuilder.Property(n => n.LastName);
        });
        builder.OwnsOne(p => p.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.City);
            addressBuilder.Property(a => a.Country);
        });
        builder.Property(p => p.Summary);
        builder.Property(p => p.Skills)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(p => p.Languages)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(p => p.Experiences)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(p => p.Educations)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(p => p.PersonalProjects)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(p => p.Certifications)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(p => p.Publications)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(p => p.Awards)
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
