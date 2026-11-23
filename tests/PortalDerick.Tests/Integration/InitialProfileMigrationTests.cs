using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDerick.Application.Profiles;
using PortalDerick.Infrastructure.Persistence;
using PortalDerick.Infrastructure.Profiles;
using Xunit;

namespace PortalDerick.Tests.Integration;

public sealed class InitialProfileMigrationTests
{
    [Fact]
    public async Task InitialMigrationSeedsExactlyOnceAndDoesNotRestoreRemovedProfile()
    {
        var token = TestContext.Current.CancellationToken;
        var path = Path.Combine(Path.GetTempPath(), $"portal-derick-spec001-{Guid.NewGuid():N}.db");
        var connection = $"Data Source={path};Pooling=False";
        var options = new DbContextOptionsBuilder<PortalDerickDbContext>().UseSqlite(connection).Options;
        try
        {
            await using (var context = new PortalDerickDbContext(options))
            {
                await context.Database.MigrateAsync(token);
                var profile = await context.ProfessionalProfiles.SingleAsync(token);
                Assert.Equal("Nome do Dev", profile.Name);
                Assert.Equal("Dev. de Software", profile.Title);
                Assert.Null(profile.Bio);
                Assert.Null(profile.Photo);
                Assert.Null(profile.Country);
                Assert.Null(profile.State);
                Assert.Single(await context.Database.GetAppliedMigrationsAsync(token));
            }
            // O host usa o mesmo arquivo já preparado; não cria esquema nem seed.
            await using var factory = new MigrationApiFactory(connection);
            using (var client = factory.CreateClient())
            {
                for (var i = 0; i < 2; i++)
                {
                    using var response = await client.GetAsync("/api/perfil", token);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
                    Assert.Equal("Nome do Dev", json.RootElement.GetProperty("nome").GetString());
                    Assert.Equal("Dev. de Software", json.RootElement.GetProperty("titulo").GetString());
                    foreach (var name in new[] { "bio", "foto", "pais", "estado" })
                        Assert.Equal(JsonValueKind.Null, json.RootElement.GetProperty(name).ValueKind);
                }
            }
            await using (var context = new PortalDerickDbContext(options))
            {
                Assert.Equal(1, await context.ProfessionalProfiles.CountAsync(token));
                await context.ProfessionalProfiles.ExecuteDeleteAsync(token);
                await context.Database.MigrateAsync(token);
                Assert.Empty(await context.ProfessionalProfiles.ToListAsync(token));
            }
            await factory.DisposeAsync();
            await using var restarted = new MigrationApiFactory(connection);
            using var restartedClient = restarted.CreateClient();
            using var absent = await restartedClient.GetAsync("/api/perfil", token);
            Assert.Equal(HttpStatusCode.NotFound, absent.StatusCode);
            Assert.Empty(await absent.Content.ReadAsByteArrayAsync(token));
        }
        finally
        {
            foreach (var suffix in new[] { "", "-wal", "-shm", "-journal" })
                File.Delete(path + suffix);
        }
    }

    private sealed class MigrationApiFactory(string connection) : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
            => builder.UseSetting("ConnectionStrings:PortalDerick", connection);
    }
}
