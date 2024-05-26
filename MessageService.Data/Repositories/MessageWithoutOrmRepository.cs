using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageService.Core.Entities;
using MessageService.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Data.Repositories;

public class MessageWithoutOrmRepository: IMessageRepository
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

    public void DeleteMessage(Message message) =>
        _dataContext.Database.ExecuteSqlInterpolated(
            $@"DELETE FROM ""Message""
                WHERE ""Id"" = {message.Id}");

    public void UpdateMessage(Message message) =>
        _dbSet.FromSqlInterpolated(
            $@"SUPDATE ""Message"" SET ""ModifiedOn"" = {DateTime.UtcNow}, ""Number"" = {message.Number}, ""Text"" = {message.Text}
            WHERE ""Id"" = {message.Id}");

    public Task<Message> GetMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = _dbSet.FromSqlInterpolated(
            $@"SELECT *
                FROM ""Message"" AS m
                WHERE m.""Id"" = {id}
                LIMIT 1").FirstAsync(cancellationToken);
        return message;
    }

    public async Task<(IEnumerable<Message>, int)> GetMessagesPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = _dbSet.Count();
        var message = await _dbSet.FromSqlInterpolated(
            $@"SELECT *
                FROM ""Message"" AS m
                LIMIT {pageSize} OFFSET {pageIndex}""")
            .ToListAsync(cancellationToken: cancellationToken);
        return (message, count);
    }

    public async Task<IEnumerable<Message>> GetMessagesListAsync(CancellationToken cancellationToken = default)
    {
        var message = await _dbSet.FromSqlInterpolated(
            $@"SELECT *
                FROM ""Message""").ToListAsync(cancellationToken: cancellationToken);
        return message;
    }
}