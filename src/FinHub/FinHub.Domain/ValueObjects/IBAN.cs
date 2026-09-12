using System.Text.RegularExpressions;

namespace FinHub.Domain.ValueObjects;

public partial record IBAN
{
    public string Value { get; }

    public IBAN(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("IBAN cannot be empty.", nameof(value));

        var sanitized = value.Replace(" ", "").ToUpperInvariant();
        if (!IbanRegex().IsMatch(sanitized))
            throw new ArgumentException($"Invalid IBAN format: '{value}'.", nameof(value));

        Value = sanitized;
    }

    [GeneratedRegex(@"^[A-Z]{2}\d{2}[A-Z0-9]{11,30}$")]
    private static partial Regex IbanRegex();

    public override string ToString() => Value;
}
