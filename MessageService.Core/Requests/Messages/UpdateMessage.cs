using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MediatR;
using MessageService.Abstractions;
using MessageService.Abstractions.Messages;

namespace MessageService.Core.Requests.Messages;

public class UpdateMessage : IRequest<MessageModel>
{
    public Guid Id { get; set; }

    [DisplayName("")]
    public DateTime CreatedOn { get; set; }
    [DisplayName("")]
    public DateTime ModifiedOn { get; set; }
    [DisplayName("")]
    public Guid? CreatedBy { get; set; }
    [DisplayName("")]
    public Guid? ModifiedBy { get; set; }
    [DisplayName("Texts")]
    public string Text { get; set; }
    [DisplayName("Номер сообщения")]
    public int Number { get; set; }
}
