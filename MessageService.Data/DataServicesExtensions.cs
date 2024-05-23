using System;
using Aeb.DigitalPlatform.Infrastructure;
using MessageService.Core.Infrastructure.Options;
using Aeb.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageService.Data;

public static class DataServicesExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        // Automatically Register Repository Dependencies
        services.Scan(scan => 
            scan.FromApplicationDependencies(a => 
                    a.FullName != null && a.FullName.StartsWith("MessageService"))
                .AddClasses(classes => classes
                    .Where(type => type.Namespace is "MessageService.Data.Repositories"))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        return services;
    }

    public static IServiceCollection AddApplicationDatabase(this IServiceCollection services, AppOptions appOptions,
        IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(options =>
        {
            switch (appOptions.Database.DatabaseEngine)
            {
                case DatabaseEngine.InMemory:
                    var inMemoryDatabaseName = appOptions.IsSut ?
                        $"MessageService-{Guid.NewGuid()}" : "MessageService";
                    options.UseInMemoryDatabase(inMemoryDatabaseName)
                        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    break;
                case DatabaseEngine.PostgreSql:
                    options.UseNpgsql(appOptions.Database.GetConnectionString(),
                        o => o.MigrationsAssembly("MessageService.Data"));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported value in field {nameof(DatabaseEngine)}");
            }
        });
        return services;
    }
}