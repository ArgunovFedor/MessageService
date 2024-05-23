using System;
using MessageService.Abstractions;

namespace MessageService.Abstractions.Messages;

public class MessageFilter : EntityFilter
{
    public DateTime? CreatedOnFrom { get; set; }
    public DateTime? CreatedOnTo { get; set; }
    public DateTime? ModifiedOnFrom { get; set; }
    public DateTime? ModifiedOnTo { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public string Text { get; set; }
    public int? NumberFrom { get; set; }
    public int? NumberTo { get; set; }
}
