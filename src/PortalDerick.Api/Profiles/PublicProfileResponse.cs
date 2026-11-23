using System.Text.Json.Serialization;
using PortalDerick.Domain;

namespace PortalDerick.Api.Profiles;

public sealed record PublicProfileResponse(
    [property: JsonPropertyName("nome")] string Nome,
    [property: JsonPropertyName("titulo")] string Titulo,
    [property: JsonPropertyName("bio"), JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Bio,
    [property: JsonPropertyName("foto"), JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Foto,
    [property: JsonPropertyName("pais"), JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Pais,
    [property: JsonPropertyName("estado"), JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Estado)
{
    public static PublicProfileResponse From(ProfessionalProfile profile)
        => new(profile.Name, profile.Title, profile.Bio, profile.Photo, profile.Country, profile.State);
}
