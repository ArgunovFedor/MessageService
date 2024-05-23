using Microsoft.Extensions.Options;

namespace MessageService.Core.UnitTests;

public class DumbOptions<T> : IOptions<T> where T : class, new()
{
    public DumbOptions(T value)
    {
        Value = value;
    }

    public T Value { get; }
}