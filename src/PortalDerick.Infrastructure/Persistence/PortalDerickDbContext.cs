using Microsoft.EntityFrameworkCore;
using PortalDerick.Domain;

namespace PortalDerick.Infrastructure.Persistence;

public sealed class PortalDerickDbContext : DbContext
{
    public PortalDerickDbContext(DbContextOptions<PortalDerickDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProfessionalProfile> ProfessionalProfiles => Set<ProfessionalProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var profile = modelBuilder.Entity<ProfessionalProfile>();

        profile.ToTable("ProfessionalProfiles", table =>
            table.HasCheckConstraint("CK_ProfessionalProfiles_Id", "Id = 1"));

        profile.HasKey(entity => entity.Id);
        profile.Property(entity => entity.Id)
            .HasColumnType("INTEGER")
            .ValueGeneratedNever();

        // Mapear todas as propriedades somente de leitura permite ao EF usar o
        // único construtor da entidade, preservando a validação na materialização.
        profile.Property(entity => entity.Name)
            .HasColumnType("TEXT")
            .IsRequired();
        profile.Property(entity => entity.Title)
            .HasColumnType("TEXT")
            .IsRequired();
        profile.Property(entity => entity.Bio)
            .HasColumnType("TEXT")
            .IsRequired(false);
        profile.Property(entity => entity.Photo)
            .HasColumnType("TEXT")
            .IsRequired(false);
        profile.Property(entity => entity.Country)
            .HasColumnType("TEXT")
            .IsRequired(false);
        profile.Property(entity => entity.State)
            .HasColumnType("TEXT")
            .IsRequired(false);
    }
}
