using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MessageService.Core.Repositories;
using MessageService.Abstractions.Messages;
using MessageService.Core.Entities;


using MediatR;
using MessageService.Abstractions;
using MessageService.Core.Infrastructure;

namespace MessageService.Core.Requests.Messages;

public class UpdateMessageHandler : IRequestHandler<UpdateMessage, MessageModel>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateMessageHandler(
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MessageModel> Handle(UpdateMessage request, CancellationToken cancellationToken)
    {
        var message = await  _messageRepository.GetMessageAsync(request.Id, cancellationToken);

        if (message == null)
        {
            throw new ServiceException("NOT_FOUND"); 
        }

        _mapper.Map(request, message);
        message.ModifiedOn = DateTime.UtcNow;
        _messageRepository.UpdateMessage(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Message, MessageModel>(message);
    }
}
