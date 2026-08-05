using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WatchLog.Application.Auth;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Controllers;

[EnableRateLimiting("auth")]
public class AuthController(IAuthService authService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResult>> Register(RegisterRequest request, CancellationToken ct) =>
        Ok(await authService.RegisterAsync(request, ct));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginRequest request, CancellationToken ct) =>
        Ok(await authService.LoginAsync(request, ct));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResult>> Refresh(RefreshRequest request, CancellationToken ct) =>
        Ok(await authService.RefreshAsync(request, ct));

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request, CancellationToken ct)
    {
        await authService.LogoutAsync(CurrentUserId, request.RefreshToken, ct);
        return NoContent();
    }

    /// <summary>Kicks off the OAuth handshake for a given provider (google/microsoft/facebook/apple).</summary>
    [HttpGet("external/{provider}/challenge")]
    public IActionResult ExternalChallenge(string provider, [FromQuery] string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalCallback), new { provider, returnUrl }) ?? "/";
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, MapScheme(provider));
    }

    /// <summary>
    /// The provider redirects here after consent. We read the short-lived "External" cookie
    /// principal the OAuth handler produced, mint our own JWT pair, and hand it back.
    /// </summary>
    [HttpGet("external/{provider}/callback")]
    public async Task<ActionResult<AuthResult>> ExternalCallback(string provider, [FromQuery] string? returnUrl = null, CancellationToken ct = default)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("External");
        if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
        {
            return Unauthorized();
        }

        var principal = authenticateResult.Principal;
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var name = principal.FindFirstValue(ClaimTypes.Name) ?? email;
        var providerKey = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        await HttpContext.SignOutAsync("External");

        if (email is null || providerKey is null)
        {
            return Unauthorized();
        }

        var result = await authService.ExternalLoginAsync(new ExternalLoginRequest(provider, providerKey, email, name ?? email), ct);
        return Ok(result);
    }

    private static string MapScheme(string provider) => provider.ToLowerInvariant() switch
    {
        "google" => Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme,
        "microsoft" => Microsoft.AspNetCore.Authentication.MicrosoftAccount.MicrosoftAccountDefaults.AuthenticationScheme,
        "facebook" => Microsoft.AspNetCore.Authentication.Facebook.FacebookDefaults.AuthenticationScheme,
        "apple" => AspNet.Security.OAuth.Apple.AppleAuthenticationDefaults.AuthenticationScheme,
        _ => throw new Application.Common.ConflictException($"Unknown provider '{provider}'.")
    };
}
