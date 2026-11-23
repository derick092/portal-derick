using PortalDerick.Domain;
using Xunit;

namespace PortalDerick.Tests.Unit;

public sealed class ProfessionalProfileTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\r\n")]
    public void RejectsInvalidRequiredValues(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(() => new ProfessionalProfile(1, value!, "Título"));
        Assert.ThrowsAny<ArgumentException>(() => new ProfessionalProfile(1, "Nome", value!));
    }

    [Fact]
    public void PreservesValuesAndIndependentOptionals()
    {
        var profile = new ProfessionalProfile(1, " Nome ", " Título ", "", "referência", null, " Estado ");
        Assert.Equal(" Nome ", profile.Name);
        Assert.Equal(" Título ", profile.Title);
        Assert.Equal("", profile.Bio);
        Assert.Equal("referência", profile.Photo);
        Assert.Null(profile.Country);
        Assert.Equal(" Estado ", profile.State);
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProfessionalProfile(2, "Nome", "Título"));
    }
}
