using FluentValidation;
using LearningTracker.Application.Configuration.Pipelines;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LearningTracker.Application;

public static class DependencyInjection {
    public static void AddApplication(this IServiceCollection services) {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddValidatorsFromAssembly(assembly);
    }
}