using System.Text.RegularExpressions;

namespace FinHub.Domain.ValueObjects;

public partial record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email address cannot be empty.", nameof(value));

        var trimmed = value.Trim().ToLowerInvariant();
        if (!EmailRegex().IsMatch(trimmed))
            throw new ArgumentException($"Invalid email address format: '{value}'.", nameof(value));

        Value = trimmed;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    public override string ToString() => Value;
}
