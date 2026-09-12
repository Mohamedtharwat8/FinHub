using System.Text.RegularExpressions;

namespace FinHub.Domain.ValueObjects;

public partial record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty.", nameof(value));

        var trimmed = value.Trim().Replace(" ", "").Replace("-", "");
        if (!PhoneRegex().IsMatch(trimmed))
            throw new ArgumentException($"Invalid phone number format: '{value}'. Must be a valid Saudi (+9665xxxxxxxx / 05xxxxxxxx) or E.164 international phone number.", nameof(value));

        Value = FormatToE164(trimmed);
    }

    [GeneratedRegex(@"^(?:\+966|966|0)?5\d{8}$|^\+\d{10,14}$")]
    private static partial Regex PhoneRegex();

    private static string FormatToE164(string phone)
    {
        if (phone.StartsWith("05"))
            return "+966" + phone[1..];
        if (phone.StartsWith("5"))
            return "+966" + phone;
        if (phone.StartsWith("9665"))
            return "+" + phone;
        return phone;
    }

    public override string ToString() => Value;
}
