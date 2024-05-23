using System.Reflection;
using Aeb.DigitalPlatform.Infrastructure;
using MessageService.Core.Infrastructure;
using MessageService.Core.Infrastructure.Options;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageService.Core;

public static class CoreServicesExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // MediatR requests registration
        services.AddMediatR(typeof(CoreServicesExtensions).Assembly);
        
        // Request validation pipeline registration
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        
        // Automapper Configuration
        services.AddSingleton(new MapperConfiguration(cfg =>
            cfg.AddMaps(typeof(CoreServicesExtensions).Assembly)
        ).CreateMapper());

        return services;
    }

    public static IServiceCollection AddNamedCorsPolicies(
        this IServiceCollection services, IConfiguration configuration)
    {
        var appOptions = configuration.GetSection("App").Get<AppOptions>();
        services.AddCors(options =>
        {
            if (appOptions.CorsPolicies == null)
            {
                return;
            }
            foreach (var (name, policy) in appOptions.CorsPolicies)
            {
                options.AddPolicy(name, policy);
            }
        });
        return services;
    }
}