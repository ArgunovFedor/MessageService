using System;

namespace MessageService.Core.Infrastructure.Options;

public class AppDatabaseOptions
{
    public DatabaseEngine DatabaseEngine { get; set; }
    public string Host { get; set; }
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; } = 5432;
    public string Attributes { get; set; }
    public bool AffectsReadiness { get; set; } = true;
    public int? MigrationCommandTimeout { get; set; }

    public string GetConnectionString()
    {
        switch (DatabaseEngine)
        {
            case DatabaseEngine.InMemory:
                throw new InvalidOperationException();
            case DatabaseEngine.PostgreSql:
                return $"Host={Host};" +
                       $"Port={Port};" +
                       $"Database={Database};" +
                       $"Username={Username};" +
                       $"Password={Password};" +
                       $"{Attributes}";
            case DatabaseEngine.SqlServer:
                return $"Server={Host},{Port};" +
                       $"Database={Database};" +
                       $"User ID={Username};" +
                       $"Password={Password};" +
                       "MultipleActiveResultSets=true;" +
                       Attributes;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}

public enum DatabaseEngine
{
    InMemory,
    PostgreSql,
    SqlServer
}