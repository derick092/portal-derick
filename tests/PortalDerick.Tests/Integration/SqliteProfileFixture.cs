using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PortalDerick.Domain;
using PortalDerick.Infrastructure.Persistence;

namespace PortalDerick.Tests.Integration;

/// <summary>
/// Criar uma instância por teste. A conexão mantenedora preserva o banco
/// exclusivo em memória entre contextos e reinícios do host.
/// </summary>
public sealed class SqliteProfileFixture : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteProfileFixture()
    {
        ConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = $"profile-{Guid.NewGuid():N}",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared,
            Pooling = false
        }.ToString();

        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();
    }

    public string ConnectionString { get; }

    public PortalDerickDbContext CreateContext()
    {
        ObjectDisposedException.ThrowIf(_connection.State != System.Data.ConnectionState.Open, this);

        var options = new DbContextOptionsBuilder<PortalDerickDbContext>()
            .UseSqlite(ConnectionString)
            .Options;

        return new PortalDerickDbContext(options);
    }

    /// <summary>
    /// Preparação explícita para testes de persistência, sem migração ou seed.
    /// Não usar nos testes de migração: estes devem partir de banco sem esquema
    /// e só aplicar migrações após autorização específica.
    /// </summary>
    public async Task CreateSchemaAsync(CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task InsertProfileAsync(
        ProfessionalProfile profile,
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();
        context.ProfessionalProfiles.Add(profile);
        await context.SaveChangesAsync(cancellationToken);
    }

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
