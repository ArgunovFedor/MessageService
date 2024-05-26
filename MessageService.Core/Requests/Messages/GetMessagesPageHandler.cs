using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using MessageService.Core.Repositories;
using MessageService.Core.Entities;
using MessageService.Abstractions.Messages;
using MediatR;
using MessageService.Abstractions;

namespace MessageService.Core.Requests.Messages;

public class GetMessagesPageHandler : IRequestHandler<GetMessagesPage, PaginableContentModel<MessageModel>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessagesPageHandler(
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<PaginableContentModel<MessageModel>> Handle(GetMessagesPage request, CancellationToken cancellationToken)
    {               
        var (items, totalCount) = await _messageRepository.GetMessagesPageAsync(request.PageIndex, request.PageSize, cancellationToken);
        return new PaginableContentModel<MessageModel>(items.Select(_mapper.Map<Message, MessageModel>), totalCount, request.PageIndex);
    }
}
