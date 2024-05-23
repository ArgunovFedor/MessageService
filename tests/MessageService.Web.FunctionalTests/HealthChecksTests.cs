using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aeb.DigitalPlatform.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Prometheus;
using Xunit;

namespace MessageService.Web.FunctionalTests;

public class HealthChecksTests
{
    private readonly HttpClient _httpClient;

    public HealthChecksTests()
    {
        var webApplicationFactory = new CustomWebApplicationFactory()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices((context, services) =>
                {
                    // Adds HealthChecks overall logic
                    services.AddDigitalPlatformHealthChecks(context.Configuration);
                });
                builder.Configure(app =>
                {
                    // Adds HealthChecks endpoints
                    app.UseDigitalPlatformHealthChecks();

                    // Add Prometheus endpoint
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapMetrics();
                    });
                });
            });
        _httpClient = webApplicationFactory.CreateClient();
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/health/ready")]
    [InlineData("/health/live")]
    [InlineData("/health/startup")]
    public async Task Health_Endpoint_StatusCode_Is_Ok(string endpointUrl)
    {
        // Act
        var response = await _httpClient.GetAsync(endpointUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Metrics_Endpoint_Has_Data()
    {
        // Act
        var response = await _httpClient.GetAsync("/metrics");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
    }
}