using System.Globalization;

namespace GiftOfTheGivers.Helpers;

public static class DonationFormatting
{
    public static string FormatCertificateNumber(string certificateNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(certificateNumber);

        var normalized = certificateNumber.Trim().ToUpperInvariant();
        return normalized.StartsWith("TAX-", StringComparison.Ordinal)
            ? normalized
            : $"TAX-{normalized}";
    }

    public static string FormatDonationAmount(decimal amount, string currency)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Donation amount must be greater than zero.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        var normalizedCurrency = currency.Trim().ToUpperInvariant();
        if (normalizedCurrency.Length != 3 || !normalizedCurrency.All(char.IsLetter))
        {
            throw new ArgumentException("Currency must be a three-letter code.", nameof(currency));
        }

        return $"{normalizedCurrency} {amount.ToString("N2", CultureInfo.InvariantCulture)}";
    }

    public static decimal CalculateDonationTotal(IEnumerable<decimal> amounts)
    {
        ArgumentNullException.ThrowIfNull(amounts);

        var total = 0m;
        foreach (var amount in amounts)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amounts), "Donation amounts cannot be negative.");
            }

            total = checked(total + amount);
        }

        return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
