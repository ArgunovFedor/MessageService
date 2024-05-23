using Microsoft.Extensions.DependencyInjection;

namespace MessageService.Web.FunctionalTests;

public static class AddMockServicesExtensions
{
    public static IServiceCollection AddMockServices(this IServiceCollection services) =>
        // Add Mocks Here:
        // services.AddScoped(_ => GetServiceNameMock().Object);
        services;
}