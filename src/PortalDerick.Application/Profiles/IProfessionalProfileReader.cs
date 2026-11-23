using PortalDerick.Domain;

namespace PortalDerick.Application.Profiles;

public interface IProfessionalProfileReader
{
    Task<ProfessionalProfile?> ReadAsync(CancellationToken cancellationToken = default);
}
