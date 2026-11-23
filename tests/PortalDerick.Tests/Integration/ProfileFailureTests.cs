using System.Net;
using PortalDerick.Domain;
using Xunit;

namespace PortalDerick.Tests.Integration;

public sealed class ProfileFailureTests
{
    [Theory]
    [InlineData("Development")]
    [InlineData("Staging")]
    [InlineData("Production")]
    public async Task ReaderFailureReturnsEmpty500InEveryEnvironment(string environment)
    {
        await using var factory = new ProfileApiFactory(
            _ => Task.FromException<ProfessionalProfile?>(new InvalidOperationException("detalhe-secreto")), environment);
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/api/perfil", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }
}
