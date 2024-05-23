using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MessageService.Abstractions.Messages;

public class CreateMessageModel
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public string Text { get; set; }
    public int Number { get; set; }
}
