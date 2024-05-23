using System.Threading;
using System.Threading.Tasks;
using Aeb.UnitOfWork.Abstractions;

namespace MessageService.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;

    public UnitOfWork(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = new()) =>
        await _dataContext.SaveChangesAsync(cancellationToken);
}