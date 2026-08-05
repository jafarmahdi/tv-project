namespace WatchLog.Application.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResult> RefreshAsync(RefreshRequest request, CancellationToken ct = default);
    Task LogoutAsync(Guid userId, string refreshToken, CancellationToken ct = default);
    Task<AuthResult> ExternalLoginAsync(ExternalLoginRequest request, CancellationToken ct = default);

    /// <summary>Issues a fresh JWT pair for an already-known user id — used by the passkey sign-in flow,
    /// which authenticates the user itself (via WebAuthn) before this is ever called.</summary>
    Task<AuthResult> IssueTokensForUserIdAsync(Guid userId, CancellationToken ct = default);
}
