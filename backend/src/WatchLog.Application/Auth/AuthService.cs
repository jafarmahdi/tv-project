using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Auth;

public class AuthService(IIdentityService identityService, ITokenService tokenService, IUnitOfWork unitOfWork)
    : IAuthService
{
    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var result = await identityService.CreateUserAsync(request.Email, request.Password, request.DisplayName, request.Locale);
        if (!result.Succeeded || result.UserId is null)
        {
            throw new ConflictException(string.Join(" ", result.Errors));
        }

        await SeedBuiltInListsAsync(result.UserId.Value, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return await IssueTokensAsync(result.UserId.Value, request.Email, ct);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var userId = await identityService.ValidateCredentialsAsync(request.Email, request.Password);
        if (userId is null)
        {
            throw new ConflictException("Invalid email or password.");
        }

        return await IssueTokensAsync(userId.Value, request.Email, ct);
    }

    public async Task<AuthResult> RefreshAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var hash = tokenService.HashRefreshToken(request.RefreshToken);
        var repo = unitOfWork.Repository<RefreshToken>();
        var existing = await repo.Query().FirstOrDefaultAsync(rt => rt.TokenHash == hash, ct);

        if (existing is null || !existing.IsActive)
        {
            throw new ForbiddenException("Refresh token is invalid or has expired.");
        }

        existing.RevokedAt = DateTimeOffset.UtcNow;

        var user = await identityService.GetUserAsync(existing.UserId)
            ?? throw new NotFoundException("User", existing.UserId);

        var result = await IssueTokensAsync(user.Id, user.Email, ct);
        existing.ReplacedByTokenHash = tokenService.HashRefreshToken(result.RefreshToken);
        repo.Update(existing);
        await unitOfWork.SaveChangesAsync(ct);

        return result;
    }

    public async Task LogoutAsync(Guid userId, string refreshToken, CancellationToken ct = default)
    {
        var hash = tokenService.HashRefreshToken(refreshToken);
        var repo = unitOfWork.Repository<RefreshToken>();
        var existing = await repo.Query().FirstOrDefaultAsync(rt => rt.TokenHash == hash && rt.UserId == userId, ct);
        if (existing is null) return;

        existing.RevokedAt = DateTimeOffset.UtcNow;
        repo.Update(existing);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<AuthResult> ExternalLoginAsync(ExternalLoginRequest request, CancellationToken ct = default)
    {
        var userId = await identityService.FindOrCreateExternalUserAsync(
            request.Provider, request.ProviderKey, request.Email, request.DisplayName);

        if (await unitOfWork.Repository<UserList>().Query().AnyAsync(l => l.UserId == userId, ct) is false)
        {
            await SeedBuiltInListsAsync(userId, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        return await IssueTokensAsync(userId, request.Email, ct);
    }

    public async Task<AuthResult> IssueTokensForUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await identityService.GetUserAsync(userId) ?? throw new NotFoundException("User", userId);
        return await IssueTokensAsync(user.Id, user.Email, ct);
    }

    private async Task<AuthResult> IssueTokensAsync(Guid userId, string email, CancellationToken ct)
    {
        var roles = await identityService.GetRolesAsync(userId);
        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(userId, email, roles);
        var (rawRefreshToken, refreshHash) = tokenService.GenerateRefreshToken();

        await unitOfWork.Repository<RefreshToken>().AddAsync(new RefreshToken
        {
            UserId = userId,
            TokenHash = refreshHash,
            ExpiresAt = DateTimeOffset.UtcNow.Add(tokenService.RefreshTokenLifetime)
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResult(userId, accessToken, expiresAt, rawRefreshToken);
    }

    private async Task SeedBuiltInListsAsync(Guid userId, CancellationToken ct)
    {
        var repo = unitOfWork.Repository<UserList>();
        foreach (var type in new[]
                 {
                     ListType.Watching, ListType.Completed, ListType.Planned,
                     ListType.OnHold, ListType.Dropped, ListType.Favorites
                 })
        {
            await repo.AddAsync(new UserList { UserId = userId, Name = type.ToString(), Type = type, IsPublic = true }, ct);
        }
    }
}
