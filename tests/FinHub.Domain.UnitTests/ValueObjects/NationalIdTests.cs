using FinHub.Domain.ValueObjects;

namespace FinHub.Domain.UnitTests.ValueObjects;

public class NationalIdTests
{
    [Theory]
    [InlineData("1000000008")] // Valid citizen ID with Modulus 10 checksum
    [InlineData("2000000006")] // Valid resident Iqama with Modulus 10 checksum
    public void ValidNationalId_ShouldInstantiate(string rawId)
    {
        // Act
        var nationalId = new NationalId(rawId);

        // Assert
        Assert.Equal(rawId, nationalId.Value);
        Assert.True(nationalId.IsCitizen || nationalId.IsResident);
    }

    [Theory]
    [InlineData("3012345674")] // Invalid starting digit
    [InlineData("101234567")]  // Only 9 digits
    [InlineData("1012345670")] // Invalid Luhn checksum
    public void InvalidNationalId_ShouldThrowArgumentException(string rawId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new NationalId(rawId));
    }
}
