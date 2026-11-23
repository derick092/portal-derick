using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortalDerick.Application.Profiles;
using PortalDerick.Domain;
using PortalDerick.Infrastructure.Persistence;
using Xunit;

namespace PortalDerick.Tests.Integration;

public sealed class ProfileTestInfrastructureTests
{
    [Theory]
    [InlineData("Development")]
    [InlineData("Staging")]
    [InlineData("Production")]
    public async Task FunctionalFactoryUsesRequestedEnvironmentWithoutPersistence(string environment)
    {
        await using var factory = new ProfileApiFactory(environment: environment);
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(environment, factory.Services.GetRequiredService<IHostEnvironment>().EnvironmentName);
        Assert.Null(factory.Services.GetService<PortalDerickDbContext>());
        Assert.Null(await factory.Services.GetRequiredService<IProfessionalProfileReader>().ReadAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SimulatedReaderPreservesCompleteAndPartialProfiles()
    {
        foreach (var profile in new[]
        {
            new ProfessionalProfile(1, "Nome", "Título", "Bio", "foto", "País", "Estado"),
            new ProfessionalProfile(1, "Nome", "Título", state: "Estado")
        })
        {
            await using var factory = new ProfileApiFactory(profile);
            var reader = factory.Services.GetRequiredService<IProfessionalProfileReader>();

            Assert.Same(profile, await reader.ReadAsync(TestContext.Current.CancellationToken));
        }
    }

    [Fact]
    public async Task SimulatedReaderPropagatesFailureAndCancellation()
    {
        var failure = new InvalidOperationException("Falha simulada.");
        await using var failingFactory = new ProfileApiFactory(_ => Task.FromException<ProfessionalProfile?>(failure));
        var failingReader = failingFactory.Services.GetRequiredService<IProfessionalProfileReader>();
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => failingReader.ReadAsync(TestContext.Current.CancellationToken)));

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await using var factory = new ProfileApiFactory();
        var reader = factory.Services.GetRequiredService<IProfessionalProfileReader>();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reader.ReadAsync(cancellation.Token));
    }

    [Fact]
    public async Task SqliteFixturesStartWithoutSchemaAndDoNotShareData()
    {
        await using var first = new SqliteProfileFixture();
        await using var second = new SqliteProfileFixture();
        Assert.NotEqual(first.ConnectionString, second.ConnectionString);

        await using (var context = second.CreateContext())
        {
            var tables = await context.Database.SqlQueryRaw<int>(
                "SELECT COUNT(*) AS Value FROM sqlite_master WHERE name = 'ProfessionalProfiles'").SingleAsync(TestContext.Current.CancellationToken);
            Assert.Equal(0, tables);
        }

        await first.CreateSchemaAsync(TestContext.Current.CancellationToken);
        await first.InsertProfileAsync(new ProfessionalProfile(1, "Nome", "Título"), TestContext.Current.CancellationToken);
        await second.CreateSchemaAsync(TestContext.Current.CancellationToken);

        await using var firstContext = first.CreateContext();
        await using var secondContext = second.CreateContext();
        Assert.Equal(1, await firstContext.ProfessionalProfiles.CountAsync(TestContext.Current.CancellationToken));
        Assert.Empty(await secondContext.ProfessionalProfiles.ToListAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HostRestartPreservesFixtureDatabaseAndReappliesServiceConfiguration()
    {
        await using var fixture = new SqliteProfileFixture();
        var registrations = 0;
        await using var original = ProfileApiFactory.ForPersistence(fixture, services =>
        {
            registrations++;
            services.AddSingleton<IProfessionalProfileReader>(new UnusedReader());
        }, "Staging");

        using (var scope = original.Services.CreateScope())
        {
            var contextBeforePreparation = scope.ServiceProvider.GetRequiredService<PortalDerickDbContext>();
            Assert.Equal(0, await contextBeforePreparation.Database.SqlQueryRaw<int>(
                "SELECT COUNT(*) AS Value FROM sqlite_master WHERE name = 'ProfessionalProfiles'")
                .SingleAsync(TestContext.Current.CancellationToken));

            await fixture.CreateSchemaAsync(TestContext.Current.CancellationToken);
            await fixture.InsertProfileAsync(new ProfessionalProfile(1, "Nome", "Título"), TestContext.Current.CancellationToken);
            Assert.Equal(1, await scope.ServiceProvider.GetRequiredService<PortalDerickDbContext>()
                .ProfessionalProfiles.CountAsync(TestContext.Current.CancellationToken));
        }

        await using var restarted = await original.RestartAsync();
        using var restartedScope = restarted.Services.CreateScope();
        var context = restartedScope.ServiceProvider.GetRequiredService<PortalDerickDbContext>();
        Assert.Equal("Nome", (await context.ProfessionalProfiles.SingleAsync(TestContext.Current.CancellationToken)).Name);
        Assert.Equal("Staging", restarted.Services.GetRequiredService<IHostEnvironment>().EnvironmentName);
        Assert.Equal(2, registrations);
    }

    // Apenas verifica a composição de serviços; o leitor real pertence à T012.
    private sealed class UnusedReader : IProfessionalProfileReader
    {
        public Task<ProfessionalProfile?> ReadAsync(CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Este teste não consulta o leitor.");
    }
}
