using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using PortalDerick.Api.Profiles;
using PortalDerick.Application.Profiles;
using PortalDerick.Infrastructure.Persistence;
using PortalDerick.Infrastructure.Profiles;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddScoped<GetPublicProfile>();
builder.Services.AddScoped<IProfessionalProfileReader, ProfessionalProfileReader>();
builder.Services.AddDbContext<PortalDerickDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("PortalDerick")
        ?? throw new InvalidOperationException("Configure ConnectionStrings:PortalDerick externamente.")));
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, token) =>
        {
            document.Info.Title = "Consulta pública do perfil profissional";
            document.Info.Description = "Consulta anônima, somente leitura, em português brasileiro.";
            return Task.CompletedTask;
        });
        options.AddSchemaTransformer((schema, context, token) =>
        {
            if (context.JsonTypeInfo.Type == typeof(PublicProfileResponse))
            {
                schema.Required = new HashSet<string>(["nome", "titulo", "bio", "foto", "pais", "estado"]);
                schema.Description = "Dados públicos do perfil; os quatro campos opcionais permanecem explícitos quando nulos.";
                schema.Example = JsonNode.Parse("""{"nome":"Nome do Dev","titulo":"Dev. de Software","bio":null,"foto":null,"pais":null,"estado":null}""");
            }
            return Task.CompletedTask;
        });
    });
}
var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
    {
        context.Abort();
    }
    catch (Exception)
    {
        app.Logger.LogError(new EventId(1, "PublicProfileFailure"),
            "Falha ao processar a consulta pública. Identificador: {TraceIdentifier}", context.TraceIdentifier);
        if (context.Response.HasStarted)
        {
            context.Abort();
            return;
        }
        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentLength = 0;
    }
});
app.MapPublicProfile();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/scalar");
}
app.Run();

public partial class Program
{
}
