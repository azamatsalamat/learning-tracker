using System.Reflection;
using LearningTracker.Application.Configuration.Pipelines;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LearningTracker.Application;

public static class DependencyInjection {
    public static void AddApplication(this IServiceCollection services) {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddValidationBehaviors(assembly);
    }
    
    private static void AddValidationBehaviors (this IServiceCollection services, Assembly assembly) {
        var validationBehaviors = assembly.GetTypes()
            .Where(t => !t.IsAbstract && 
                        t.BaseType?.IsGenericType == true && 
                         (t.BaseType.GetGenericTypeDefinition() == typeof(ValidationBehavior<>) ||
                          t.BaseType.GetGenericTypeDefinition() == typeof(ValidationBehavior<,>)))
            .ToList();

        foreach (var behaviorType in validationBehaviors)
        {
            var interfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
                .ToList();
            
            foreach (var interfaceType in interfaces)
            {
                services.AddScoped(interfaceType, behaviorType);
            }
        }
    }
}