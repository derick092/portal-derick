using PortalDerick.Domain;

namespace PortalDerick.Application.Profiles;

public sealed class GetPublicProfile(IProfessionalProfileReader reader)
{
    public Task<ProfessionalProfile?> ExecuteAsync(CancellationToken cancellationToken = default)
        => reader.ReadAsync(cancellationToken);
}
