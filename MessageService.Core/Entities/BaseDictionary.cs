using System.ComponentModel.DataAnnotations;

namespace MessageService.Core.Entities;

public abstract class BaseDictionary : BaseEntity
{
    [StringLength(100)]
    public string Name { get; set; }
}