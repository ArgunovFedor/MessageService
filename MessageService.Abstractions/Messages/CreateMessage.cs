using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MessageService.Abstractions.Messages;

public class CreateMessageModel
{
   
    [DisplayName("Texts")]
    public string Text { get; set; }
    [DisplayName("Номер сообщения")]
    public int Number { get; set; }
}
