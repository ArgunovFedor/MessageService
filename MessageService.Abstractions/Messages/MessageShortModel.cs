using System;
using System.Collections.Generic;

namespace MessageService.Abstractions.Messages;

public class MessageShortModel
{
    public Guid Id { get; set; }

    public string Text { get; set; }
}
