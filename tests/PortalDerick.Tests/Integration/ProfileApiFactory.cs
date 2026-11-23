using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortalDerick.Application.Profiles;
using PortalDerick.Domain;
using PortalDerick.Infrastructure.Persistence;

namespace PortalDerick.Tests.Integration;

public sealed class ProfileApiFactory : WebApplicationFactory<Program>
{
    private readonly string _environment;
    private readonly Func<CancellationToken, Task<ProfessionalProfile?>>? _readProfile;
    private readonly SqliteProfileFixture? _persistence;
    private readonly Action<IServiceCollection>? _configureReader;

    public ProfileApiFactory(ProfessionalProfile? profile = null, string environment = "Development")
        : this(token =>
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(profile);
        }, environment)
    {
    }

    public ProfileApiFactory(
        Func<CancellationToken, Task<ProfessionalProfile?>> readProfile,
        string environment = "Development")
        : this(environment, readProfile, null, null)
    {
        ArgumentNullException.ThrowIfNull(readProfile);
    }

    private ProfileApiFactory(
        string environment,
        Func<CancellationToken, Task<ProfessionalProfile?>>? readProfile,
        SqliteProfileFixture? persistence,
        Action<IServiceCollection>? configureReader)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(environment);
        _environment = environment;
        _readProfile = readProfile;
        _persistence = persistence;
        _configureReader = configureReader;
    }

    // O registro do leitor real será fornecido pelos testes após T012.
    // A factory nunca cria esquema, aplica migrações ou prepara dados.
    public static ProfileApiFactory ForPersistence(
        SqliteProfileFixture persistence,
        Action<IServiceCollection> configureReader,
        string environment = "Development")
    {
        ArgumentNullException.ThrowIfNull(persistence);
        ArgumentNullException.ThrowIfNull(configureReader);
        return new ProfileApiFactory(environment, null, persistence, configureReader);
    }

    public async Task<ProfileApiFactory> RestartAsync()
    {
        await DisposeAsync();
        return new ProfileApiFactory(_environment, _readProfile, _persistence, _configureReader);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        if (_persistence is not null)
        {
            builder.UseSetting("ConnectionStrings:PortalDerick", _persistence.ConnectionString);
        }

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IProfessionalProfileReader>();
            services.RemoveAll<PortalDerickDbContext>();
            services.RemoveAll<DbContextOptions<PortalDerickDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<PortalDerickDbContext>>();

            if (_persistence is null)
            {
                services.AddSingleton<IProfessionalProfileReader>(new SimulatedReader(_readProfile!));
            }
            else
            {
                services.AddDbContext<PortalDerickDbContext>(options =>
                    options.UseSqlite(_persistence.ConnectionString));
                _configureReader!(services);
            }
        });
    }

    private sealed class SimulatedReader(
        Func<CancellationToken, Task<ProfessionalProfile?>> readProfile) : IProfessionalProfileReader
    {
        public Task<ProfessionalProfile?> ReadAsync(CancellationToken cancellationToken = default)
            => readProfile(cancellationToken);
    }
}
