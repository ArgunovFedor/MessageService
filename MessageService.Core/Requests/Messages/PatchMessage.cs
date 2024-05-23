using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
using MediatR;
using MessageService.Core.Entities;
using MessageService.Abstractions.Messages;

namespace MessageService.Core.Requests.Messages;

public class PatchMessage : IRequest<MessageModel>
{
    public PatchMessage()
    {
    }

    public PatchMessage(Guid id, JsonPatchDocument<MessageModel> content)
    {
        Id = id;
        Content = content;
    }

    public Guid Id { get; set; }

    public JsonPatchDocument<MessageModel> Content { get; set; }
}
