using System.Threading;
using System.Threading.Tasks;
using MessageService.Core.Repositories;


using MediatR;
using MessageService.Abstractions;
using MessageService.Core.Infrastructure;

namespace MessageService.Core.Requests.Messages;

public class DeleteMessageHandler : IRequestHandler<DeleteMessage>
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
    public async Task<Unit> Handle(DeleteMessage request, CancellationToken cancellationToken)
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
