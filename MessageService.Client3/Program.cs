using MessageService.Client3;
using MessageService.Connector;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prometheus;
using Refit;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:7166");
builder.WebHost.UseKestrel(options => options.AllowSynchronousIO = true);

var services = builder.Services;
var configuration = builder.Configuration;
// AppOptions
services.AddOptions();
// Web
services.AddHttpContextAccessor();
services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

// FluentValidation
services.AddFluentValidationRulesToSwagger();

// Swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options => options.ConfigureSwagger("MessageService"));
services.AddSwaggerGenNewtonsoftSupport();

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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Client3");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Client3 API Docs";
});
app.UseReDoc(c =>
{
    c.SpecUrl("/swagger/v1/swagger.json");
    c.RoutePrefix = "redoc";
    c.DocumentTitle = "Client3 API Docs";
});

var serverUrl = configuration.GetSection("MessageServiceWebUrl").Get<string>();
var refitClient = RestService.For<IMessageRestClient>(serverUrl);
app.MapGet("/getMessages", async () =>
    {
        var result = await refitClient.GetPushSoundsAsync(DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow);
        return Results.Json(result);
    })
    .WithMetadata(new SwaggerOperationAttribute(summary: "getMessages", description: "Сообщения за последние 10 минут"))
    .WithMetadata("feds")
    .WithDisplayName("fede");

await app.RunAsync();