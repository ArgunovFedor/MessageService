using MessageService.Connector;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5050");
var serverUrl = "http://localhost:5000";
var refitClient = RestService.For<IMessageRestClient>(serverUrl);
var app = builder.Build();
app.MapGet("/getMessages", async () =>
{
    var result = await refitClient.GetPushSoundsAsync(DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow);
    return Results.Json(result);
});


await app.RunAsync();