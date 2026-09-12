using System.Collections.Concurrent;
using System.Security.Cryptography;
using FinHub.Application.Common.Interfaces;

namespace FinHub.Infrastructure.Security;

public sealed class OtpService : IOtpService
{
    private sealed record OtpEntry(string Code, DateTimeOffset Expiry, int Attempts);

    private static readonly ConcurrentDictionary<string, OtpEntry> OtpStore = new();

    public Task<string> GenerateOtpAsync(string target, string purpose, CancellationToken cancellationToken = default)
    {
        var key = GetKey(target, purpose);

        // Generate 6-digit cryptographically secure numeric OTP
        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var expiry = DateTimeOffset.UtcNow.AddMinutes(5);

        OtpStore[key] = new OtpEntry(code, expiry, 0);

        return Task.FromResult(code);
    }

    public Task<bool> VerifyOtpAsync(string target, string code, string purpose, CancellationToken cancellationToken = default)
    {
        var key = GetKey(target, purpose);
        if (!OtpStore.TryGetValue(key, out var entry))
        {
            return Task.FromResult(false);
        }

        if (entry.Expiry < DateTimeOffset.UtcNow)
        {
            OtpStore.TryRemove(key, out _);
            return Task.FromResult(false);
        }

        if (entry.Attempts >= 3)
        {
            OtpStore.TryRemove(key, out _);
            return Task.FromResult(false);
        }

        if (entry.Code == code.Trim())
        {
            OtpStore.TryRemove(key, out _);
            return Task.FromResult(true);
        }

        // Increment failed attempt count
        OtpStore[key] = entry with { Attempts = entry.Attempts + 1 };
        return Task.FromResult(false);
    }

    private static string GetKey(string target, string purpose) => $"{purpose.ToLowerInvariant()}:{target.ToLowerInvariant().Trim()}";
}
