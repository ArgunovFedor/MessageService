using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MessageService.Core.Entities;
using MessageService.Core.Infrastructure;
using MessageService.Abstractions;
using MessageService.Abstractions.Messages;
using MediatR;

namespace MessageService.Core.Repositories;

/// <summary>
/// Message Repository interface
/// </summary>
public interface IMessageRepository
{
    /// <summary>
    /// Insert new record of Message
    /// </summary>
    /// <param name="message"></param>
    void AddMessage(Message message);

    /// <summary>
    /// Delete a record of Message
    /// </summary>
    /// <param name="message"></param>
    void DeleteMessage(Message message);

    /// <summary>
    /// Update a record of Message
    /// </summary>
    /// <param name="message"></param>
    void UpdateMessage(Message message);

    /// <summary>
    /// Get a record of Message
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message> GetMessageAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Messages collection with pagination
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Requested page and total count</returns>
    Task<(IEnumerable<Message>, int)> GetMessagesPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Messages collection
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<Message>> GetMessagesListAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Message>> GetMessagesListBetweenDatesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
