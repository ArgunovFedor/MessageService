using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MessageService.Core.Repositories;
using MessageService.Core.Entities;
using MessageService.Core.Infrastructure;
using MessageService.Abstractions.Messages;

using MediatR;

namespace MessageService.Core.Requests.Messages;

public class GetMessageHandler : IRequestHandler<GetMessage, MessageModel>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessageHandler(
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<MessageModel> Handle(GetMessage request, CancellationToken cancellationToken)
    {                       
        var message = await _messageRepository
            .GetMessageAsync(request.Id, cancellationToken);

        if (message == null)
        {
            throw new ServiceException("NOT_FOUND"); 
        }

        return _mapper.Map<Message, MessageModel>(message);
    }
}
