using System.Security.Cryptography;
using System.Text;
using FinHub.Application.Common.Interfaces;

namespace FinHub.Infrastructure.Security;

public sealed class TotpService : ITotpService
{
    private const int Step = 30; // 30-second TOTP window

    public string GenerateSecret()
    {
        var buffer = RandomNumberGenerator.GetBytes(20);
        return Convert.ToBase64String(buffer);
    }

    public bool VerifyCode(string secret, string code)
    {
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code) || code.Length != 6)
            return false;

        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        for (int window = -1; window <= 1; window++)
        {
            var counter = (currentTime / Step) + window;
            var expectedCode = ComputeTotp(secret, counter);
            if (expectedCode == code)
                return true;
        }

        return false;
    }

    private static string ComputeTotp(string secret, long counter)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(counterBytes);

        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(counterBytes);

        int offset = hash[^1] & 0x0F;
        int binaryCode = ((hash[offset] & 0x7F) << 24) |
                         ((hash[offset + 1] & 0xFF) << 16) |
                         ((hash[offset + 2] & 0xFF) << 8) |
                         (hash[offset + 3] & 0xFF);

        int otp = binaryCode % 1_000_000;
        return otp.ToString("D6");
    }
}
