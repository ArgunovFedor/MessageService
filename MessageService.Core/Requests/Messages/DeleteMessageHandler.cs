using System.Threading;
using System.Threading.Tasks;
using MessageService.Core.Repositories;
using Aeb.DigitalPlatform.Infrastructure;
using Aeb.UnitOfWork.Abstractions;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class DeleteMessageHandler : BaseRequestHandler<DeleteMessage>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteMessageHandler(
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }
    public override async Task<Unit> HandleAsync(DeleteMessage request, CancellationToken cancellationToken)
    {              
        var message = await _messageRepository
            .GetMessageAsync(request.Id, cancellationToken);

        if (message == null)
        {
            throw new ServiceException("NOT_FOUND"); 
        }

        _messageRepository.DeleteMessage(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
