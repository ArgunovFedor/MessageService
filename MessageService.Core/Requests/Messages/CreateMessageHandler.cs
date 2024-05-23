using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Aeb.DigitalPlatform.Infrastructure;
using MessageService.Abstractions.Messages;
using MessageService.Core.Repositories;
using MessageService.Core.Entities;
using Aeb.UnitOfWork.Abstractions;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class CreateMessageHandler : BaseRequestHandler<CreateMessage, MessageModel>
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
    public override async Task<MessageModel> HandleAsync(CreateMessage request, CancellationToken cancellationToken)
    {
        var message = _mapper.Map<CreateMessage, Message>(request);
        _messageRepository.AddMessage(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // get and return updated data
        message = await  _messageRepository.GetMessageAsync(message.Id, cancellationToken);
        return _mapper.Map<Message, MessageModel>(message);
    }
}
