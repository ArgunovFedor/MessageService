using System;
using System.Collections.Generic;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class GetMessages : IRequest<IEnumerable<MessageModel>>
{
    public DateTime? SelectStart { get; set; }
    public DateTime? SelectEnd { get; set; }
        
}
