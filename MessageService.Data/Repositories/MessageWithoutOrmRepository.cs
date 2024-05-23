using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageService.Core.Entities;
using MessageService.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Data.Repositories;

public class MessageWithoutOrmRepository
{
    private readonly DbSet<Message> _dbSet;
    private readonly DataContext _dataContext;
    /// <summary>
    /// Default constructor fo DI
    /// </summary>
    /// <param name="dataContext"></param>
    public MessageWithoutOrmRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
        _dbSet = dataContext.Set<Message>();
    }
    public void AddMessage(Message message)
    {
        _dataContext.Database.ExecuteSqlInterpolated(
            $@"INSERT INTO ""Message"" 
                (""Id"", ""CreatedBy"", ""CreatedOn"", 
                 ""ModifiedBy"", ""ModifiedOn"", ""Number"", 
                 ""Text"")
                VALUES ({message.Id}, {message.CreatedBy}, {message.CreatedOn}, 
                {message.ModifiedBy}, {message.ModifiedOn}, {message.Number}, 
                {message.Text});");
    }

    public void DeleteMessage(Message message) => throw new NotImplementedException();

    public void UpdateMessage(Message message) => throw new NotImplementedException();

    public Task<Message> GetMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = _dbSet.FromSqlInterpolated(
            $@"SELECT *
                FROM ""Message"" AS m
                WHERE m.""Id"" = {id}
                LIMIT 1").FirstAsync(cancellationToken);
        return message;
    }

    public Task<(IEnumerable<Message>, int)> GetMessagesPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<IEnumerable<Message>> GetMessagesListAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}