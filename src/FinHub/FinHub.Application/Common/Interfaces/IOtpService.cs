namespace FinHub.Application.Common.Interfaces;

public interface IOtpService
{
    Task<string> GenerateOtpAsync(string target, string purpose, CancellationToken cancellationToken = default);
    Task<bool> VerifyOtpAsync(string target, string code, string purpose, CancellationToken cancellationToken = default);
}
