using System.Net;
using Microsoft.Extensions.DependencyInjection;
using PortalDerick.Application.Profiles;
using PortalDerick.Domain;
using PortalDerick.Infrastructure.Profiles;
using Xunit;

namespace PortalDerick.Tests.Integration;

public sealed class AbsentProfileTests
{
    [Fact]
    public async Task EmptyDatabaseStaysAbsentAcrossRestartButNullOptionalsArePresent()
    {
        var token = TestContext.Current.CancellationToken;
        await using var fixture = new SqliteProfileFixture();
        await fixture.CreateSchemaAsync(token);
        await using var original = ProfileApiFactory.ForPersistence(fixture,
            services => services.AddScoped<IProfessionalProfileReader, ProfessionalProfileReader>());
        using (var client = original.CreateClient())
        using (var response = await client.GetAsync("/api/perfil", token))
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Empty(await response.Content.ReadAsByteArrayAsync(token));
        }
        await using var restarted = await original.RestartAsync();
        using var restartedClient = restarted.CreateClient();
        using var absent = await restartedClient.GetAsync("/api/perfil", token);
        Assert.Equal(HttpStatusCode.NotFound, absent.StatusCode);
        Assert.Empty(await absent.Content.ReadAsByteArrayAsync(token));
        await fixture.InsertProfileAsync(new ProfessionalProfile(1, "Nome", "Título"), token);
        using var present = await restartedClient.GetAsync("/api/perfil", token);
        Assert.Equal(HttpStatusCode.OK, present.StatusCode);
    }
}
