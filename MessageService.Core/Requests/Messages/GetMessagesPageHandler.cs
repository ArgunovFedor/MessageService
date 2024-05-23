using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aeb.DigitalPlatform.Infrastructure;
using AutoMapper;
using MessageService.Core.Repositories;
using MessageService.Core.Entities;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class GetMessagesPageHandler : BaseRequestHandler<GetMessagesPage, PaginableContentModel<MessageModel>>
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

    public override async Task<PaginableContentModel<MessageModel>> HandleAsync(GetMessagesPage request, CancellationToken cancellationToken)
    {               
        var (items, totalCount) = await _messageRepository.GetMessagesPageAsync(request.PageIndex, request.PageSize, cancellationToken);
        return new PaginableContentModel<MessageModel>(items.Select(_mapper.Map<Message, MessageModel>), totalCount, request.PageIndex);
    }
}
