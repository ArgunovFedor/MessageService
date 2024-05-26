using System.Threading.Tasks;

using MessageService.Core.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MessageService.Data;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly AppOptions _appOptions;
    private readonly DataContext _dataContext;

    public DatabaseInitializer(DataContext dataContext, IOptions<AppOptions> appOptions)
    {
        _dataContext = dataContext;
        _appOptions = appOptions.Value;
    }

    public async Task SeedAsync()
    {
        if (_appOptions.Database.DatabaseEngine == DatabaseEngine.InMemory)
        {
            return;
        }

        await _dataContext.Database.MigrateAsync();
    }
}