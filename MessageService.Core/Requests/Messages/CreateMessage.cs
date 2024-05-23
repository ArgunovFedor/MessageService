using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessageService.Abstractions;
using MessageService.Core.Infrastructure;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class CreateMessage : IRequest<MessageModel>
{
    [DisplayName("Texts")]
    public string Text { get; set; }
    [DisplayName("Номер сообщения")]
    public int Number { get; set; }
}
