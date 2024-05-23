using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MessageService.Web.FunctionalTests;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Костыль для перезаписи конфигурации (втч и локальной),
        // возможно пофиксят в поздних версиях .NET
        // Issue: https://github.com/dotnet/aspnetcore/issues/37680
        builder.UseEnvironment("AutoTests");
        builder.ConfigureHostConfiguration( configBuilder => 
        { 
            configBuilder.AddJsonFile("appsettings.AutoTests.json", optional: true);
        });
        return base.CreateHost(builder);
    }
}