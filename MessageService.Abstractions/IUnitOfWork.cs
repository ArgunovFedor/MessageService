﻿using System.Threading;
using System.Threading.Tasks;

namespace MessageService.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
