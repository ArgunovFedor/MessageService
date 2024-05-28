using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using MessageService.Core.Infrastructure;
using MessageService.Core.Infrastructure.Options;
using Microsoft.Extensions.Options;


namespace MessageService.Core.Services;

public class ClientWebSocketProxy : IClientWebSocketProxy
{
    private static Mutex mutex = new Mutex();
    private static ClientWebSocket _instance;
    public WebSocketServer SocketServer { get; }

    public ClientWebSocketProxy(IOptions<WebSocketServer> options)
    {
        SocketServer = options.Value;
    }
    private ClientWebSocket ClientWebSocket
    {
        get
        {
            if (_instance == null)
            {
                mutex.WaitOne(); // Ожидаем получения мьютекса
                try
                {
                    if (_instance == null)
                    {
                        _instance = new ClientWebSocket();
                    }
                }
                finally
                {
                    mutex.ReleaseMutex(); // Освобождаем мьютекс после создания экземпляра
                }
            }
            else
            {
                if (_instance.State != WebSocketState.Connecting && _instance.State != WebSocketState.Open && _instance.State != WebSocketState.None && _instance.State != WebSocketState.CloseSent)
                {
                    mutex.WaitOne(); 
                    try
                    {
                        _instance = new ClientWebSocket();
                    }
                    finally
                    {
                        mutex.ReleaseMutex(); // Освобождаем мьютекс после создания экземпляра
                    }
                }
            }
            return _instance;
        }
    }
    public async Task<ClientWebSocket> GetWebSocketClient()
    {
        if (ClientWebSocket.State == WebSocketState.None)
        {
            await ConnectToServer(ClientWebSocket, SocketServer.Url);
        }

        return await Task.FromResult(ClientWebSocket);
    }

    private async Task ConnectToServer(ClientWebSocket client, string serverUri)
    {
        try
        {
            await client.ConnectAsync(new Uri(serverUri), CancellationToken.None);
        }
        catch (Exception ex)
        {
            throw new ServiceException($"Ошибка подключения к прокси серверу - {SocketServer.Url}");
        }
    }
}