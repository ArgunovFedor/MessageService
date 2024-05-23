using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MessageService.Core.Repositories;
using MessageService.Core.Entities;
using MessageService.Abstractions.Messages;
using Aeb.DigitalPlatform.Infrastructure;
using Aeb.UnitOfWork.Abstractions;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class PatchMessageHandler : BaseRequestHandler<PatchMessage, MessageModel>
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

    public override async Task<MessageModel> HandleAsync(PatchMessage request, CancellationToken cancellationToken)
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
        _messageRepository.UpdateMessage(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return obj;
    }
}
