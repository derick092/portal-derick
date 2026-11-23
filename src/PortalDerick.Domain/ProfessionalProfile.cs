namespace PortalDerick.Domain;

public sealed class ProfessionalProfile
{
    public ProfessionalProfile(
        int id,
        string name,
        string title,
        string? bio = null,
        string? photo = null,
        string? country = null,
        string? state = null)
    {
        if (id != 1)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "A identidade do perfil deve ser 1.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Id = id;
        Name = name;
        Title = title;
        Bio = bio;
        Photo = photo;
        Country = country;
        State = state;
    }

    public int Id { get; }

    public string Name { get; }

    public string Title { get; }

    public string? Bio { get; }

    public string? Photo { get; }

    public string? Country { get; }

    public string? State { get; }
}
