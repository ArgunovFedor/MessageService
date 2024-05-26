using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageService.Core.Entities;
using MessageService.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessageService.Data.Repositories;

/// <inheritdoc/>
public class MessageRepository 
{
    private readonly DbSet<Message> _dbSet;

    /// <summary>
    /// Default constructor fo DI
    /// </summary>
    /// <param name="dataContext"></param>
    public MessageRepository(DataContext dataContext)
    {
        _dbSet = dataContext.Set<Message>();
    }

    /// <inheritdoc/>
    public void AddMessage(Message message)
    {
        _dbSet.Add(message);
    }

    /// <inheritdoc/>
    public void DeleteMessage(Message message)
    {
        _dbSet.Remove(message);
    }

    /// <inheritdoc/>
    public void UpdateMessage(Message message)
    {
        _dbSet.Update(message);
    }

    /// <inheritdoc/>
    public async Task<Message> GetMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await _dbSet
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return message;
    }

    /// <inheritdoc/>
    public async Task<(IEnumerable<Message>, int)> GetMessagesPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);
        var messages = await _dbSet
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (messages, totalCount);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Message>> GetMessagesListAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _dbSet
            .ToListAsync(cancellationToken);
        return messages;
    }
}
