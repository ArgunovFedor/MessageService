using System;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class DeleteMessage : IRequest
{
    public Guid Id { get; set; }

    public DeleteMessage()
    {
    }

    public DeleteMessage(Guid id)
    {
        Id = id;
    }
}
