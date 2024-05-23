using System;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class GetMessage : IRequest<MessageModel>
{
    public Guid Id { get; set; }

    public GetMessage()
    {
    }

    public GetMessage(Guid id)
    {
        Id = id;
    }
}
