using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PortalDerick.Domain;
using PortalDerick.Infrastructure.Profiles;
using Xunit;

namespace PortalDerick.Tests.Integration;

public sealed class ProfilePersistenceTests
{
    [Fact]
    public async Task SqliteEnforcesCardinalityAndReaderDoesNotTrackOrWrite()
    {
        var token = TestContext.Current.CancellationToken;
        await using var fixture = new SqliteProfileFixture();
        await fixture.CreateSchemaAsync(token);
        await fixture.InsertProfileAsync(new ProfessionalProfile(1, " Nome ", "Título", state: "Estado"), token);
        await using var context = fixture.CreateContext();
        await Assert.ThrowsAsync<SqliteException>(() => context.Database.ExecuteSqlRawAsync(
            "INSERT INTO ProfessionalProfiles (Id, Name, Title) VALUES (1, 'Outro', 'Título')", token));
        await Assert.ThrowsAsync<SqliteException>(() => context.Database.ExecuteSqlRawAsync(
            "INSERT INTO ProfessionalProfiles (Id, Name, Title) VALUES (2, 'Outro', 'Título')", token));
        var reader = new ProfessionalProfileReader(context);
        for (var i = 0; i < 2; i++)
        {
            var profile = await reader.ReadAsync(token);
            Assert.Equal(" Nome ", profile?.Name);
            Assert.Null(profile?.Country);
            Assert.Equal("Estado", profile?.State);
            Assert.Empty(context.ChangeTracker.Entries());
        }
        Assert.Equal(1, await context.ProfessionalProfiles.CountAsync(token));
        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reader.ReadAsync(cancelled.Token));
    }

    [Theory]
    [InlineData("Name", "")]
    [InlineData("Name", "   ")]
    [InlineData("Title", "")]
    [InlineData("Title", "   ")]
    public async Task MaterializationUsesValidatedConstructor(string column, string value)
    {
        var token = TestContext.Current.CancellationToken;
        await using var fixture = new SqliteProfileFixture();
        await fixture.CreateSchemaAsync(token);
        await fixture.InsertProfileAsync(new ProfessionalProfile(1, "Nome", "Título"), token);
        await using var context = fixture.CreateContext();
        var sql = column == "Name" ? "UPDATE ProfessionalProfiles SET Name = {0}" : "UPDATE ProfessionalProfiles SET Title = {0}";
        await context.Database.ExecuteSqlRawAsync(sql, [value], token);
        await Assert.ThrowsAnyAsync<ArgumentException>(() => context.ProfessionalProfiles.AsNoTracking().SingleAsync(token));
    }
}
