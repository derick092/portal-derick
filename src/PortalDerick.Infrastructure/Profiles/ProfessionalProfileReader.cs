using Microsoft.EntityFrameworkCore;
using PortalDerick.Application.Profiles;
using PortalDerick.Domain;
using PortalDerick.Infrastructure.Persistence;

namespace PortalDerick.Infrastructure.Profiles;

public sealed class ProfessionalProfileReader(PortalDerickDbContext context) : IProfessionalProfileReader
{
    public Task<ProfessionalProfile?> ReadAsync(CancellationToken cancellationToken = default)
        => context.ProfessionalProfiles.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
}
