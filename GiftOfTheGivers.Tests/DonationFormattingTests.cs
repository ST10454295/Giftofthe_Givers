using GiftOfTheGivers.Helpers;
using Xunit;

namespace GiftOfTheGivers.Tests;

public class DonationFormattingTests
{
    [Theory]
    [InlineData("tax-abc123", "TAX-ABC123")]
    [InlineData(" abc123 ", "TAX-ABC123")]
    public void FormatCertificateNumber_NormalizesPrefixAndCase(string input, string expected)
    {
        Assert.Equal(expected, DonationFormatting.FormatCertificateNumber(input));
    }

    [Fact]
    public void FormatDonationAmount_UsesThreeLetterCurrencyAndTwoDecimals()
    {
        Assert.Equal("ZAR 1,250.50", DonationFormatting.FormatDonationAmount(1250.5m, "zar"));
    }

    [Fact]
    public void CalculateDonationTotal_ReturnsRoundedSum()
    {
        Assert.Equal(35.56m, DonationFormatting.CalculateDonationTotal([10.111m, 25.444m]));
    }

    [Fact]
    public void CalculateDonationTotal_RejectsNegativeAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DonationFormatting.CalculateDonationTotal([10m, -1m]));
    }
}
