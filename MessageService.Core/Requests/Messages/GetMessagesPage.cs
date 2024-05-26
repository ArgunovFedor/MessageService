
using MessageService.Abstractions.Messages;
using MediatR;
using MessageService.Abstractions;

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
