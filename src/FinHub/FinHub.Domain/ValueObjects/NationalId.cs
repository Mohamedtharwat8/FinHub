using System.Text.RegularExpressions;

namespace FinHub.Domain.ValueObjects;

public partial record NationalId
{
    public string Value { get; }
    public bool IsCitizen => Value.StartsWith('1');
    public bool IsResident => Value.StartsWith('2');

    public NationalId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("National ID / Iqama cannot be empty.", nameof(value));

        var trimmed = value.Trim();
        if (!SaudiIdRegex().IsMatch(trimmed))
            throw new ArgumentException($"Invalid Saudi National ID or Iqama format: '{value}'. Must be 10 digits starting with 1 or 2.", nameof(value));

        if (!IsValidLuhn(trimmed))
            throw new ArgumentException($"Invalid Saudi National ID or Iqama checksum: '{value}'.", nameof(value));

        Value = trimmed;
    }

    [GeneratedRegex(@"^[12]\d{9}$")]
    private static partial Regex SaudiIdRegex();

    private static bool IsValidLuhn(string id)
    {
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            int digit = id[i] - '0';
            if (i % 2 == 0)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }
            sum += digit;
        }
        return sum % 10 == 0;
    }

    public override string ToString() => Value;
}
