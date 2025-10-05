using Microsoft.EntityFrameworkCore;

namespace LearningTracker.Database;

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
}
