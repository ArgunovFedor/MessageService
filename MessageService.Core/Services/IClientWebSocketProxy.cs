using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MessageService.Core.Services;

public interface IClientWebSocketProxy
{
    Task<ClientWebSocket> GetWebSocketClient();
}