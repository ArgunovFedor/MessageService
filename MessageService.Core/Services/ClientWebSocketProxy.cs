﻿using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MessageService.Core.Services;

public class ClientWebSocketProxy : IClientWebSocketProxy
{
    private static Mutex mutex = new Mutex();
    private static ClientWebSocket _instance;

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

            return _instance;
        }
    }

    public async Task<ClientWebSocket> GetWebSocketClient()
    {
        if (ClientWebSocket.State != WebSocketState.Open || ClientWebSocket.State != WebSocketState.Connecting)
        {
            await ConnectToServer(ClientWebSocket, $"ws://localhost:6969/ws?name=Test");
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
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}