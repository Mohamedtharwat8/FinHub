using System.Text.RegularExpressions;

namespace FinHub.Domain.ValueObjects;

public partial record IBAN
{
    public string Value { get; }

    public IBAN(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("IBAN cannot be empty.", nameof(value));

        var trimmed = value.Trim().Replace(" ", "").ToUpperInvariant();

        if (!IbanRegex().IsMatch(trimmed))
            throw new ArgumentException($"Invalid Saudi IBAN format: '{value}'. Must start with 'SA' followed by 22 alphanumeric characters.", nameof(value));

        if (!ValidateMod97(trimmed))
            throw new ArgumentException($"Invalid IBAN checksum: '{value}'.", nameof(value));

        Value = trimmed;
    }

    [GeneratedRegex(@"^SA\d{2}[A-Z0-9]{18}\d{2}$|^SA\d{22}$")]
    private static partial Regex IbanRegex();

    private static bool ValidateMod97(string iban)
    {
        var rearranged = iban[4..] + iban[..4];
        var numericIban = string.Concat(rearranged.Select(c => char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

        int checksum = 0;
        foreach (char digitChar in numericIban)
        {
            checksum = (checksum * 10 + (digitChar - '0')) % 97;
        }

        return checksum == 1;
    }

    public static IBAN GenerateSaudiIban(string accountNumber)
    {
        var cleanAcc = accountNumber.PadLeft(18, '0')[..18];
        var bban = "80" + cleanAcc; // 80 = Al-Rajhi / SAMA bank code prefix
        var tempStr = bban + "281000"; // SA = 2810 in numeric representation
        int remainder = 0;
        foreach (char c in tempStr)
        {
            remainder = (remainder * 10 + (c - '0')) % 97;
        }
        int checkDigits = 98 - remainder;
        var checkStr = checkDigits.ToString("D2");
        return new IBAN("SA" + checkStr + bban);
    }

    public override string ToString() => Value;
}
