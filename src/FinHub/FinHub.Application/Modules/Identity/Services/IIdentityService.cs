using FinHub.Application.Modules.Identity.DTOs;

namespace FinHub.Application.Modules.Identity.Services;

public interface IIdentityService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> ExternalLoginAsync(ExternalLoginRequest request, CancellationToken cancellationToken = default);
    Task<string> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default);
    Task<bool> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterWithPhoneAsync(PhoneRegisterRequest request, CancellationToken cancellationToken = default);
}
