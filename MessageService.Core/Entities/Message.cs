using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MessageService.Abstractions;

namespace MessageService.Core.Entities;

[Table("Message")]
public class Message : BaseEntity
{
    [Required()]
    [StringLength(128)]
    public string Text { get; set; }
    
    [Required()]
    public int Number { get; set; }
    
}
