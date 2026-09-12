namespace FinHub.Application.Common.Interfaces;

public interface ITotpService
{
    string GenerateSecret();
    bool VerifyCode(string secret, string code);
}
