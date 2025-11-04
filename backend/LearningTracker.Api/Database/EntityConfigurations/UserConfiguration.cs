using LearningTracker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningTracker.Database.EntityConfigurations;

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
        builder.HasOne(x => x.Profile)
            .WithOne(x => x.User)
            .IsRequired(false)
            .HasForeignKey<Profile>(x => x.Id)
            .HasPrincipalKey<User>(x => x.Id);
    }
}
