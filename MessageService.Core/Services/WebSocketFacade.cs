using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using MessageService.Abstractions.Messages;

namespace MessageService.Core.Services;

public class WebSocketFacade: IWebSocketFacade<MessageModel>
{
    private readonly IClientWebSocketProxy ClientWebSocketProxy;
    public WebSocketFacade(IClientWebSocketProxy clientWebSocketProxy)
    {
        ClientWebSocketProxy = clientWebSocketProxy;
    }
    public async Task SendAsync(MessageModel model)
    {
        var client = await ClientWebSocketProxy.GetWebSocketClient();
        if (client.State == WebSocketState.Open)
        {
            var bytesToSend =
                new ArraySegment<byte>(
                    System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(model)));
            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}