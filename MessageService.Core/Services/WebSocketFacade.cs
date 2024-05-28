using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using MessageService.Abstractions.Messages;
using MessageService.Core.Infrastructure;
using MessageService.Core.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace MessageService.Core.Services;

public class WebSocketFacade : IWebSocketFacade<MessageModel>
{
    // private readonly IClientWebSocketProxy ClientWebSocketProxy;
    public WebSocketServer SocketServer { get; }

    public WebSocketFacade(IOptions<WebSocketServer> options)
    {
        SocketServer = options.Value;
    }

    public async Task SendAsync(MessageModel model)
    {
        // var client = await ClientWebSocketProxy.GetWebSocketClient();
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri(SocketServer.Url), CancellationToken.None);
        try
        {
            if (client.State == WebSocketState.Open)
            {
                var bytesToSend =
                    new ArraySegment<byte>(
                        System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(model)));
                await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            Console.WriteLine("WebSocket connection closed prematurely.");
        }
    }
}