using System.Net;
using System.Text.Json;
using PortalDerick.Domain;
using Xunit;

namespace PortalDerick.Tests.Integration;

public sealed class ApiDocumentationTests
{
    [Fact]
    public async Task DevelopmentDocumentationDescribesContractWithoutReadingProfile()
    {
        await using var factory = new ProfileApiFactory(_ => throw new InvalidOperationException("Não consultar dados para documentar."));
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var root = json.RootElement;
        var operation = root.GetProperty("paths").GetProperty("/api/perfil").GetProperty("get");
        Assert.False(operation.TryGetProperty("security", out _));
        Assert.False(root.TryGetProperty("security", out _));
        Assert.Equal("Consultar o perfil profissional", operation.GetProperty("summary").GetString());
        var responses = operation.GetProperty("responses");
        Assert.Equal(new[] { "200", "404", "500" }, responses.EnumerateObject().Select(p => p.Name).Order());
        Assert.False(responses.GetProperty("404").TryGetProperty("content", out _));
        Assert.False(responses.GetProperty("500").TryGetProperty("content", out _));
        var schema = root.GetProperty("components").GetProperty("schemas").GetProperty("PublicProfileResponse");
        Assert.Equal(new[] { "bio", "estado", "foto", "nome", "pais", "titulo" },
            schema.GetProperty("required").EnumerateArray().Select(p => p.GetString()).Order());
        Assert.Equal(6, schema.GetProperty("properties").EnumerateObject().Count());
        foreach (var name in new[] { "bio", "foto", "pais", "estado" })
            Assert.Contains("null", schema.GetProperty("properties").GetProperty(name).GetProperty("type").EnumerateArray().Select(p => p.GetString()));
        foreach (var name in new[] { "nome", "titulo" })
            Assert.Equal("string", schema.GetProperty("properties").GetProperty(name).GetProperty("type").GetString());
        Assert.Equal("Nome do Dev", schema.GetProperty("example").GetProperty("nome").GetString());
        Assert.Equal("Dev. de Software", schema.GetProperty("example").GetProperty("titulo").GetString());
        using var scalar = await client.GetAsync("/scalar", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, scalar.StatusCode);
        var html = await scalar.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("openapi/v1.json", html);
        Assert.Contains("scalar", html, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Development")]
    [InlineData("Staging")]
    [InlineData("Production")]
    public async Task RoutesFollowEnvironmentWhileProfileRemainsAvailable(string environment)
    {
        await using var factory = new ProfileApiFactory(new ProfessionalProfile(1, "Nome", "Título"), environment);
        using var client = factory.CreateClient();
        using var profile = await client.GetAsync("/api/perfil", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
        foreach (var path in new[] { "/openapi/v1.json", "/scalar", "/scalar/v1", "/scalar/scalar.js", "/scalar/scalar.aspnetcore.js" })
        {
            using var response = await client.GetAsync(path, TestContext.Current.CancellationToken);
            Assert.Equal(environment == "Development" ? HttpStatusCode.OK : HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("PATCH")]
    [InlineData("DELETE")]
    public async Task WriteMethodsAreRejectedWithoutCallingReader(string method)
    {
        await using var factory = new ProfileApiFactory(_ => throw new InvalidOperationException("Não consultar em escrita."));
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(new HttpMethod(method), "/api/perfil");
        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }
}

