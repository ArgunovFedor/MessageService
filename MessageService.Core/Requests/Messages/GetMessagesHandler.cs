using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

using MessageService.Core.Repositories;
using MessageService.Core.Entities;
using MessageService.Core.Infrastructure;
using MessageService.Abstractions;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class GetMessagesHandler : IRequestHandler<GetMessages, IEnumerable<MessageModel>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessagesHandler(
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MessageModel>> Handle(GetMessages request, CancellationToken cancellationToken)
    {               
        var items = await _messageRepository.GetMessagesListAsync(cancellationToken);
        return items.Select(_mapper.Map<Message, MessageModel>);
    }
}
