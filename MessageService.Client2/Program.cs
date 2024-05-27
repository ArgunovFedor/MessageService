using System.Net;
using System.Net.WebSockets;
using System.Text;
using MessageService.Abstractions.Messages;
using MessageService.Client2.Infrastructure;
using Newtonsoft.Json;

// второй клиент при считывает по веб-сокету поток сообщений от сервера и отображает их в порядке прихода с сервера (с отображением метки времени и порядкового номера)

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options => options.AllowSynchronousIO = true)
    .ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            configurationBuilder.Configure(context, args);
        }
    );

var app = builder.Build();
app.UseWebSockets();
var connections = new List<WebSocket>();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        var curName = "test";
        connections.Add(ws);
        await Broadcast($"{connections.Count} users connected");
        await ReceiveMessage(ws,
            async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    // Десериализация JSON
                    var message = JsonConvert.DeserializeObject<MessageModel>(data);
                    Console.WriteLine($"{DateTime.UtcNow} - {message.Text} {message.Number}");
                    await Broadcast(curName + ": " + message);
                }
                else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                {
                    connections.Remove(ws);
                    await Broadcast($"{curName} left the room");
                    await Broadcast($"{connections.Count} users connected");
                    await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                        CancellationToken.None);
                }
            });
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

async Task Broadcast(string message)
{
    var bytes = Encoding.UTF8.GetBytes(message);
    foreach (var socket in connections)
    {
        if (socket.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

await app.RunAsync();