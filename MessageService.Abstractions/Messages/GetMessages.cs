using System;
using MessageService.Abstractions;

namespace MessageService.Abstractions.Messages;

public class GetMessagesModel
{
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
}
