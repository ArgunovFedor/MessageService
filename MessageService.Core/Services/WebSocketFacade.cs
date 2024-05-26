using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using MessageService.Abstractions.Messages;
using MessageService.Core.Infrastructure;

namespace MessageService.Core.Services;

public class WebSocketFacade : IWebSocketFacade<MessageModel>
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
            try
            {
                var bytesToSend =
                    new ArraySegment<byte>(
                        System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(model)));
                await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (System.IO.IOException ex)
            {
                // Обработка других ошибок SocketException
                client.Dispose();
                throw new ServiceException($"SocketException: {ex.Message}");
            }
        }
    }
}