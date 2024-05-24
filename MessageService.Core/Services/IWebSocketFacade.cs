using System.Threading.Tasks;

namespace MessageService.Core.Services;

public interface IWebSocketFacade<T> where T:class
{
    Task SendAsync(T model);
}