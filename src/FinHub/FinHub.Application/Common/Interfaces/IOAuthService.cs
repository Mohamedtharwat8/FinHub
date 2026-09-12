namespace FinHub.Application.Common.Interfaces;

public sealed record OAuthUserInfo(
    string Provider,
    string ProviderKey,
    string Email,
    string FullName,
    bool EmailVerified
);

public interface IOAuthService
{
    Task<OAuthUserInfo> ValidateTokenAsync(string provider, string token, CancellationToken cancellationToken = default);
}
