using System.Collections.Generic;
using MessageService.Core.Infrastructure;
using MessageService.Abstractions;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class GetMessages : IRequest<IEnumerable<MessageModel>>
{
}
