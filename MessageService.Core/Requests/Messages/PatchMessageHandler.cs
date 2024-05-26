using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MessageService.Core.Repositories;
using MessageService.Core.Entities;
using MessageService.Abstractions.Messages;


using MediatR;
using MessageService.Abstractions;
using MessageService.Core.Infrastructure;

namespace MessageService.Core.Requests.Messages;

public class PatchMessageHandler : IRequestHandler<PatchMessage, MessageModel>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PatchMessageHandler(
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MessageModel> Handle(PatchMessage request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository
            .GetMessageAsync(request.Id, cancellationToken: cancellationToken);

        if (message == null)
        {
            throw new ServiceException("NOT_FOUND"); 
        }

        var obj = _mapper.Map<Message, MessageModel>(message);
        request.Content.ApplyTo(obj);
        _mapper.Map(obj, message);
        message.ModifiedOn = DateTime.UtcNow;
        _messageRepository.UpdateMessage(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return obj;
    }
}
