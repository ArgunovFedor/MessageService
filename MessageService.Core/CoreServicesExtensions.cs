using System.Reflection;

using MessageService.Core.Infrastructure;
using MessageService.Core.Infrastructure.Options;
using AutoMapper;
using MediatR;
using MessageService.Abstractions.Messages;
using MessageService.Core.Services;
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
        services.AddSingleton<IClientWebSocketProxy, ClientWebSocketProxy>();
        services.AddTransient<IWebSocketFacade<MessageModel>, WebSocketFacade>();
        // Automapper Configuration
        services.AddSingleton(new MapperConfiguration(cfg =>
            cfg.AddMaps(typeof(CoreServicesExtensions).Assembly)
        ).CreateMapper());

        return services;
    }
}