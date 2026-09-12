using System.Security.Cryptography;
using FinHub.Application.Common.Interfaces;

namespace FinHub.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            Algorithm,
            KeySize);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        var parts = passwordHash.Split('.');
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            Algorithm,
            KeySize);

        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}
