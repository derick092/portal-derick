using System.Net;
using System.Text.Json;
using PortalDerick.Domain;
using Xunit;

namespace PortalDerick.Tests.Integration;

public sealed class PublicProfileContractTests
{
    public static TheoryData<int> OptionalCombinations => new(Enumerable.Range(0, 16));

    [Theory]
    [MemberData(nameof(OptionalCombinations))]
    public async Task AnonymousGetPreservesAllSixFields(int mask)
    {
        string? Optional(int bit, string value) => (mask & (1 << bit)) != 0 ? value : null;
        var profile = new ProfessionalProfile(1, " Nome ", "Título em português", Optional(0, "Bio"),
            Optional(1, "imagem sem formato imposto"), Optional(2, "Brasil"), Optional(3, "São Paulo"));
        await using var factory = new ProfileApiFactory(profile);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US");
        using var response = await client.GetAsync("/api/perfil", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var actual = json.RootElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString());
        var expected = new Dictionary<string, string?>
        {
            ["nome"] = profile.Name,
            ["titulo"] = profile.Title,
            ["bio"] = profile.Bio,
            ["foto"] = profile.Photo,
            ["pais"] = profile.Country,
            ["estado"] = profile.State
        };
        Assert.Equal(expected.OrderBy(p => p.Key), actual.OrderBy(p => p.Key));
    }

    [Fact]
    public async Task MissingProfileReturnsEmpty404()
    {
        await using var factory = new ProfileApiFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/api/perfil", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }
}
