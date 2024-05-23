using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MessageService.Core.Repositories;
using MessageService.Abstractions.Messages;
using MessageService.Core.Entities;
using Aeb.DigitalPlatform.Infrastructure;
using Aeb.UnitOfWork.Abstractions;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class UpdateMessageHandler : BaseRequestHandler<UpdateMessage, MessageModel>
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

    public override async Task<MessageModel> HandleAsync(UpdateMessage request, CancellationToken cancellationToken)
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
