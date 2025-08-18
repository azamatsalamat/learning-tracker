using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearningTracker.Infrastructure.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddLearningTrackerDbContext(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<LearningTrackerDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("LearningTracker.Infrastructure")));

        return services;
    }
}
