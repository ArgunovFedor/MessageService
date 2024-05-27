using MessageService.Core;
using MessageService.Core.Infrastructure.Options;
using MessageService.Data;
using MessageService.Web.Infrastructure;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureAppConfiguration((context, configurationBuilder) =>
    {
        configurationBuilder.Configure(context, args);
    }
);
builder.WebHost.UseKestrel(options => options.AllowSynchronousIO = true)
    .ConfigureLogging((_, builder) =>
    {
        builder.AddSentry();
    });

var services = builder.Services;
var configuration = builder.Configuration;

var appOptions = configuration.GetSection("App").Get<AppOptions>();

// AppOptions
services.AddOptions();
services.Configure<AppOptions>(configuration.GetSection("App"));

// DB
services.AddApplicationDatabase(appOptions, configuration);
services.AddDataServices();

// Core
services.AddCoreServices();

// Web
services.AddHttpContextAccessor();
services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });
services.AddScoped<ApiExceptionFilterAttribute>();

// FluentValidation
services.AddValidatorsFromAssembly(typeof(CoreServicesExtensions).Assembly);
services.AddFluentValidationRulesToSwagger();

// Swagger
services.AddSwaggerGen(options => options.ConfigureSwagger("MessageService"));
services.AddSwaggerGenNewtonsoftSupport();

// Register custom services below
services.AddJaeger(configuration);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MessageService");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "MessageService API Docs";
});
app.UseReDoc(c =>
{
    c.SpecUrl("/swagger/v1/swagger.json");
    c.RoutePrefix = "redoc";
    c.DocumentTitle = "MessageService API Docs";
});

// Initialize Database, run migrations etc
using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    databaseInitializer.SeedAsync().GetAwaiter().GetResult();
}

app.Run();
