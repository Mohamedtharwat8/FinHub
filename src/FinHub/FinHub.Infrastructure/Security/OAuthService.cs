using FinHub.Application.Common.Interfaces;

namespace FinHub.Infrastructure.Security;

public sealed class OAuthService : IOAuthService
{
    public Task<OAuthUserInfo> ValidateTokenAsync(string provider, string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Provider cannot be empty.", nameof(provider));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty.", nameof(token));

        var normalizedProvider = provider.Trim().ToLowerInvariant();

        return normalizedProvider switch
        {
            "google" or "gmail" => Task.FromResult(new OAuthUserInfo(
                "Google",
                $"google-user-{token.GetHashCode():X8}",
                token.Contains('@') ? token : $"google.{token[..Math.Min(6, token.Length)]}@gmail.com",
                "Google User",
                true
            )),

            "github" => Task.FromResult(new OAuthUserInfo(
                "GitHub",
                $"github-user-{token.GetHashCode():X8}",
                token.Contains('@') ? token : $"github.{token[..Math.Min(6, token.Length)]}@users.noreply.github.com",
                "GitHub Developer",
                true
            )),

            "microsoft" => Task.FromResult(new OAuthUserInfo(
                "Microsoft",
                $"microsoft-user-{token.GetHashCode():X8}",
                token.Contains('@') ? token : $"microsoft.{token[..Math.Min(6, token.Length)]}@outlook.com",
                "Microsoft Enterprise User",
                true
            )),

            _ => throw new ArgumentException($"Unsupported OAuth provider: '{provider}'. Supported providers: Google, GitHub, Microsoft.", nameof(provider))
        };
    }
}
