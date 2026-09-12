using FinHub.Application.Modules.Identity.DTOs;
using FinHub.Application.Modules.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinHub.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _identityService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _identityService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _identityService.RefreshTokenAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("external-login")]
    public async Task<ActionResult<AuthResponse>> ExternalLogin([FromBody] ExternalLoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _identityService.ExternalLoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("send-otp")]
    public async Task<ActionResult> SendOtp([FromBody] SendOtpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var otpCode = await _identityService.SendOtpAsync(request, cancellationToken);
            return Ok(new { message = "OTP sent successfully.", target = request.Target, otpDemo = otpCode });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("verify-otp")]
    public async Task<ActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        var isValid = await _identityService.VerifyOtpAsync(request, cancellationToken);
        if (isValid)
        {
            return Ok(new { verified = true, message = "OTP verified successfully." });
        }
        return BadRequest(new { verified = false, error = "Invalid or expired OTP code." });
    }

    [HttpPost("register-phone")]
    public async Task<ActionResult<AuthResponse>> RegisterWithPhone([FromBody] PhoneRegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _identityService.RegisterWithPhoneAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
