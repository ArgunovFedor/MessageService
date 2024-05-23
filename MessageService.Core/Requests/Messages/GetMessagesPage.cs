using Aeb.DigitalPlatform.Infrastructure;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class GetMessagesPage : IRequest<PaginableContentModel<MessageModel>>
{
    public GetMessagesPage(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
