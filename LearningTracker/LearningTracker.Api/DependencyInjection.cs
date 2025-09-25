using LearningTracker.Api.Configurations;
using LearningTracker.Api.Services;
using LearningTracker.Api.Services.Base;
using Microsoft.AspNetCore.Identity;

namespace LearningTracker.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection("Auth"));

        services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<TextExtractorService>();

        return services;
    }
}
