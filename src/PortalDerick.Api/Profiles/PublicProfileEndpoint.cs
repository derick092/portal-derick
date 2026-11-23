using PortalDerick.Application.Profiles;

namespace PortalDerick.Api.Profiles;

public static class PublicProfileEndpoint
{
    public static void MapPublicProfile(this WebApplication app)
    {
        app.MapGet("/api/perfil", async (GetPublicProfile query, CancellationToken cancellationToken) =>
        {
            var profile = await query.ExecuteAsync(cancellationToken);
            return profile is null ? Results.NotFound() : Results.Ok(PublicProfileResponse.From(profile));
        })
        .AllowAnonymous()
        .WithSummary("Consultar o perfil profissional")
        .WithDescription("Retorna os dados existentes do único perfil. Na ausência, retorna 404 sem corpo; falhas técnicas retornam 500 sem detalhes internos.")
        .Produces<PublicProfileResponse>()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
