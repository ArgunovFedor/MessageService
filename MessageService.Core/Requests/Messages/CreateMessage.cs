using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessageService.Abstractions;
using MessageService.Core.Infrastructure;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Requests.Messages;

public class CreateMessage : CreateMessageModel, IRequest<MessageModel>
{
}
