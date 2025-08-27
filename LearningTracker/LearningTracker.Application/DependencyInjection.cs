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
        services.AddValidatorsFromAssembly(assembly);
        
        var pipelineBehaviorTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && 
                       t.GetInterfaces().Any(i => i.IsGenericType && 
                                                 i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)));
        
        foreach (var behaviorType in pipelineBehaviorTypes) {
            var interfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
            
            foreach (var interfaceType in interfaces) {
                services.AddTransient(interfaceType, behaviorType);
            }
        }
    }
}