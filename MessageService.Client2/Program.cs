using System.Net;
using System.Net.WebSockets;
using System.Text;
using MessageService.Abstractions.Messages;
using MessageService.Client2.Infrastructure;
using Serilog;
using Newtonsoft.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options => options.AllowSynchronousIO = true)
    .ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            configurationBuilder.Configure(context, args);
        }
    )
    .ConfigureLogging(logging =>
    {
        logging.AddSerilog();
    });

var app = builder.Build();
app.UseWebSockets();
var connections = new List<WebSocket>();


app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
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
                                Log.Information($"{DateTime.UtcNow} - {message.Text} {message.Number}");
                                Console.WriteLine($"{DateTime.UtcNow} - {message.Text} {message.Number}");
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
            Console.WriteLine("WebSocket connection closed prematurely.");
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