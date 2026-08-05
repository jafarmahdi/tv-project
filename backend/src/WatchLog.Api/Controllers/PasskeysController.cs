using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Auth;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Controllers;

/// <summary>WebAuthn/passkey registration and sign-in ceremonies. See `IPasskeyService` for the flow.</summary>
public class PasskeysController(IPasskeyService passkeyService, IAuthService authService, ICurrentUserService currentUser)
    : ApiControllerBase(currentUser)
{
    public record BeginRegisterRequest(string Username, string DisplayName);
    public record CompleteRegisterRequest(string AttestationResponseJson, string OriginalOptionsJson, string? DeviceName);
    public record BeginLoginRequest(string? Username);
    public record CompleteLoginRequest(string AssertionResponseJson, string OriginalOptionsJson);

    [Authorize]
    [HttpPost("register/begin")]
    public async Task<ActionResult<string>> BeginRegister(BeginRegisterRequest request, CancellationToken ct) =>
        Ok(await passkeyService.BeginRegistrationAsync(CurrentUserId, request.Username, request.DisplayName, ct));

    [Authorize]
    [HttpPost("register/complete")]
    public async Task<IActionResult> CompleteRegister(CompleteRegisterRequest request, CancellationToken ct)
    {
        await passkeyService.CompleteRegistrationAsync(CurrentUserId, request.AttestationResponseJson, request.OriginalOptionsJson, request.DeviceName, ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("login/begin")]
    public async Task<ActionResult<string>> BeginLogin(BeginLoginRequest request, CancellationToken ct) =>
        Ok(await passkeyService.BeginAssertionAsync(request.Username, ct));

    [AllowAnonymous]
    [HttpPost("login/complete")]
    public async Task<ActionResult<AuthResult>> CompleteLogin(CompleteLoginRequest request, CancellationToken ct)
    {
        var userId = await passkeyService.CompleteAssertionAsync(request.AssertionResponseJson, request.OriginalOptionsJson, ct);
        // WebAuthn already proved who they are; just issue our normal JWT pair for this existing user.
        var result = await authService.IssueTokensForUserIdAsync(userId, ct);
        return Ok(result);
    }
}
