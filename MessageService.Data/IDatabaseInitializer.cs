using System.Threading.Tasks;

namespace MessageService.Data;

public interface IDatabaseInitializer
{
    Task SeedAsync();
}