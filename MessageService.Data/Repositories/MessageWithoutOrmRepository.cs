using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageService.Core.Entities;
using MessageService.Core.Infrastructure.Options;
using MessageService.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace MessageService.Data.Repositories;

public class MessageWithoutOrmRepository: IMessageRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// Default constructor fo DI
    /// </summary>
    /// <param name="dataContext"></param>
    /// <param name="options"></param>
    public MessageWithoutOrmRepository(IOptions<AppOptions> options)
    {
        _connectionString = options.Value.Database.GetConnectionString();
    }

    public async void AddMessage(Message message)
    {
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query = @"INSERT INTO ""Message"" (""Id"", ""CreatedBy"",""CreatedOn"", ""ModifiedBy"", ""ModifiedOn"", ""Number"", ""Text"") VALUES (@Column1, @Column2, @Column3, @Column4, @Column5, @Column6, @Column7)";

            await using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Column1", message.Id);
                command.Parameters.AddWithValue("@Column2", DBNull.Value);
                command.Parameters.AddWithValue("@Column3", message.CreatedOn);
                command.Parameters.AddWithValue("@Column4", DBNull.Value);
                command.Parameters.AddWithValue("@Column5", message.ModifiedOn);
                command.Parameters.AddWithValue("@Column6", message.Number);
                command.Parameters.AddWithValue("@Column7", message.Text);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async void DeleteMessage(Message message)
    {
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = @"DELETE FROM ""Message"" WHERE ""Id"" = @Id";

            await using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", message.Id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async void UpdateMessage(Message message)
    {
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = @"UPDATE ""Message"" SET ""ModifiedOn"" = @Column1, ""Number"" = @Column2, ""Text"" = @Column3 WHERE ""Id"" = @Id";

            await using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Column1", message.ModifiedOn);
                command.Parameters.AddWithValue("@Column2", message.Number);
                command.Parameters.AddWithValue("@Column3", message.Text); 
                command.Parameters.AddWithValue("@Id", message.Id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<Message> GetMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Message message = null;

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT * FROM ""Message"" WHERE ""Id"" = @Id";

            await using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (await reader.ReadAsync(cancellationToken))
                    {
                        message = new Message
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            Number = reader.GetInt32(reader.GetOrdinal("Number")),
                            CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                            ModifiedOn = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"))
                        };
                    }
                }
            }
        }

        return message;
    }

    public async Task<(IEnumerable<Message>, int)> GetMessagesPageAsync(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<Message>();
        var totalCount = 0;

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            // Count query
            var countQuery = @"SELECT COUNT(*) FROM ""Message""";
            await using (var countCommand = new NpgsqlCommand(countQuery, connection))
            {
                // Execute the count query
                totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken));
            }

            // Select query with pagination
            var selectQuery = @"SELECT * FROM ""Message"" OFFSET @Offset LIMIT @Limit";
            await using (var selectCommand = new NpgsqlCommand(selectQuery, connection))
            {
                selectCommand.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                selectCommand.Parameters.AddWithValue("@Limit", pageSize);

                // Execute the select query
                await using (var reader = await selectCommand.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        messages.Add(new Message
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            Number = reader.GetInt32(reader.GetOrdinal("Number")),
                            CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                            ModifiedOn = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"))
                        });
                    }
                }
            }
        }

        return (messages, totalCount);
    }

    public async Task<IEnumerable<Message>> GetMessagesListAsync(CancellationToken cancellationToken = default)
    {
        var messages = new List<Message>();

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT * FROM ""Message""";

            await using (var command = new NpgsqlCommand(query, connection))
            {
                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        messages.Add(new Message
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            Number = reader.GetInt32(reader.GetOrdinal("Number")),
                            CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                            ModifiedOn = reader.GetDateTime(reader.GetOrdinal("ModifiedOn"))
                        });
                    }
                }
            }
        }

        return messages;
    }
}