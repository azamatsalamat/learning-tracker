using LearningTracker.Application.Services;
using LearningTracker.Domain.Repositories;
using LearningTracker.Infrastructure.DataAccess;
using LearningTracker.Infrastructure.Repositories;
using LearningTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearningTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddLearningTrackerDbContext(configuration);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IUsersRepository, UsersRepository>();
        
        return services;
    }
    
    private static IServiceCollection AddLearningTrackerDbContext(
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
