using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MessageService.Web.Infrastructure;

public static class JaegerRegistrationExtensions
{
    public static IServiceCollection AddJaeger(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var serviceName = configuration.GetValue<string>("JAEGER_SERVICE_NAME");
        services.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = context =>
                        context.Request.Path.Value != null &&
                        !context.Request.Path.Value.Contains("/health") &&
                        !context.Request.Path.Value.Contains("/metrics");
                    options.RecordException = true;
                })
                .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                .AddHttpClientInstrumentation(options =>
                {
                    options.SetHttpFlavor = true;
                    options.Filter = message =>
                    {
                        var requestUriHost = message.RequestUri?.Host;
                        if (requestUriHost == null) return true;
                        var elasticNodeUris =
                            configuration.GetValue<string>("Serilog:WriteTo:ElasticSearch:Args:nodeUris");
                        var elasticUris = elasticNodeUris.Split(',').Select(s => new Uri(s));
                        return elasticUris.All(hosts => !message.RequestUri.Host.Contains(hosts.Host));
                    };
                });

            // Add optional instrumentation
            builder.AddSource("MassTransit");

            // Enrich span tags with baggage data
            builder.AddProcessor(new BaggageTagsEnrichingProcessor());

            // Export all telemetry to jaeger
            builder.AddJaegerExporter();
        });

        return services;
    }
    
    internal class BaggageTagsEnrichingProcessor : BaseProcessor<Activity>
    {
        public override void OnStart(Activity activity)
        {
            foreach (var (key, value) in activity.Baggage)
            {
                activity.SetTag(key, value);
            }
        }
    }
}