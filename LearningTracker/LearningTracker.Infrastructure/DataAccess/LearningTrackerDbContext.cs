using Microsoft.EntityFrameworkCore;
using LearningTracker.Domain.Entities;
using LearningTracker.Domain.ValueObjects.Ids;

namespace LearningTracker.Infrastructure.DataAccess;

public class LearningTrackerDbContext : DbContext
{
    public LearningTrackerDbContext(DbContextOptions<LearningTrackerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearningTrackerDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<UserId>()
            .HaveConversion<UserIdConverter>();
    }
}
