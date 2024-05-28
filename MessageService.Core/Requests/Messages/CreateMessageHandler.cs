using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

using MessageService.Abstractions.Messages;
using MessageService.Core.Repositories;
using MessageService.Core.Entities;

using MediatR;
using MessageService.Abstractions;

namespace MessageService.Core.Requests.Messages;

public class CreateMessageHandler : IRequestHandler<CreateMessage, MessageModel>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateMessageHandler(
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MessageModel> Handle(CreateMessage request, CancellationToken cancellationToken)
    {
        var message = _mapper.Map<CreateMessage, Message>(request);
        var utcNow = DateTime.UtcNow;
        message.CreatedOn = utcNow;
        message.ModifiedOn = utcNow;
        message.Id = Guid.NewGuid();
        _messageRepository.AddMessage(message);
        //_unitOfWork.SaveChanges();

        // get and return updated data
        message = await  _messageRepository.GetMessageAsync(message.Id, cancellationToken);
        return _mapper.Map<Message, MessageModel>(message);
    }
}
