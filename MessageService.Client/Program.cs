using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using MessageService.Abstractions.Messages;
using MessageService.Client.Infrastructure;
using MessageService.Connector;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prometheus;
using Refit;
using Swashbuckle.AspNetCore.Annotations;
using OpenTelemetry;


var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureAppConfiguration((context, configurationBuilder) =>
    {
        configurationBuilder.Configure(context, args);
    }
).ConfigureLogging((Action<HostBuilderContext, ILoggingBuilder>) ((_, builder) => builder.AddSentry()));
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

// Register custom services below
services.AddJaeger(configuration);

var app = builder.Build();
app.UseWebSockets();
var connections = new List<WebSocket>();

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
app.MapPost("/start", async (int count) =>
    {
        foreach (var number in Enumerable.Range(1, count))
        {
            await refitClient.CreateUserSoundSettingAsync(new CreateMessageModel()
            {
                Number = number,
                Text = $"Сообщение №{number}"
            });
        }
        return Results.Ok();
    })
    .WithMetadata(new SwaggerOperationAttribute(summary: "start",
        description: "Изначально писало 1 потоком в рамках консольного приложения, но для удобства реализовано через api"));

app.MapGet("/getMessages", async () =>
    {
        var result = await refitClient.GetPushSoundsAsync(DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow);
        return Results.Json(result);
    })
    .WithMetadata(new SwaggerOperationAttribute(summary: "getMessages",
        description: "Сообщения за последние 10 минут"));

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var services = app.Services.CreateScope().ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        connections.Add(ws);
        try
        {
            while (ws.State == WebSocketState.Open)
            {
                await ReceiveMessage(ws,
                    async (result, buffer) =>
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            // Десериализация JSON
                            var message = JsonConvert.DeserializeObject<MessageModel>(data);
                            if (message != null)
                            {
                                logger.LogInformation($"уникальный ID: {message.Id};порядковый номер: {message.Number};сообщение: {message.Text}; метки времени: {message.CreatedOn}");
                                Activity.Current?.SetSharedTag("Result", $"уникальный ID: {message.Id};порядковый номер: {message.Number};сообщение: {message.Text}; метки времени: {message.CreatedOn}");
                            }
                        }
                        else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                        {
                            await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                                CancellationToken.None);
                            connections.Remove(ws);
                        }
                    });
            }
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            logger.LogError("WebSocket connection closed prematurely.");
        }
    }
    else
    {
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
    }
});

async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
{
    var buffer = new byte[1024 * 4];
    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        handleMessage(result, buffer);
    }
}


await app.RunAsync();