using Aeb.DigitalPlatform.Infrastructure;
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
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDigitalPlatformInfrastructure(args);
builder.WebHost.UseKestrel(options => options.AllowSynchronousIO = true);

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

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
services.AddDigitalPlatformAuthentication(configuration);
services.AddNamedCorsPolicies(configuration);
services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    })
    .AddDigitalPlatformLoginEndpoint(configuration);
services.AddScoped<ApiExceptionFilterAttribute>();

// FluentValidation
services.AddValidatorsFromAssembly(typeof(CoreServicesExtensions).Assembly);
services.AddFluentValidationRulesToSwagger();

// Swagger
services.AddSwaggerGen(options => options.ConfigureSwagger("MessageService"));
services.AddSwaggerGenNewtonsoftSupport();

// Register custom services below



var app = builder.Build();

if (!appOptions.IsSut)
{
    app.UseDigitalPlatformHealthChecks();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();

app.UseWebSockets();

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
