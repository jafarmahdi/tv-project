using FluentAssertions;
using Moq;
using WatchLog.Application.Auth;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Tests.TestSupport;
using WatchLog.Domain.Entities;

namespace WatchLog.Application.Tests.Auth;

public class AuthServiceTests
{
    private readonly Mock<IIdentityService> _identity = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _tokens.Setup(t => t.RefreshTokenLifetime).Returns(TimeSpan.FromDays(30));
        _tokens.Setup(t => t.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Returns(("fake-access-token", DateTimeOffset.UtcNow.AddMinutes(15)));
        _tokens.Setup(t => t.GenerateRefreshToken()).Returns(("raw-refresh-token", "hashed-refresh-token"));

        _sut = new AuthService(_identity.Object, _tokens.Object, _unitOfWork);
    }

    [Fact]
    public async Task RegisterAsync_WhenIdentityCreationSucceeds_SeedsSixBuiltInListsAndIssuesTokens()
    {
        var userId = Guid.NewGuid();
        _identity.Setup(i => i.CreateUserAsync("new@watchlog.test", "Password1", "New User", "en"))
            .ReturnsAsync(new IdentityCreateResult(true, userId, []));
        _identity.Setup(i => i.GetRolesAsync(userId)).ReturnsAsync((IReadOnlyList<string>)[]);

        var result = await _sut.RegisterAsync(new RegisterRequest("new@watchlog.test", "Password1", "New User"));

        result.UserId.Should().Be(userId);
        result.AccessToken.Should().Be("fake-access-token");
        result.RefreshToken.Should().Be("raw-refresh-token");

        var lists = _unitOfWork.Repository<UserList>().Query().Where(l => l.UserId == userId).ToList();
        lists.Should().HaveCount(6, "every new user gets Watching/Completed/Planned/OnHold/Dropped/Favorites");

        _unitOfWork.Repository<RefreshToken>().Query().Should().ContainSingle(rt => rt.UserId == userId);
    }

    [Fact]
    public async Task RegisterAsync_WhenIdentityCreationFails_ThrowsConflictException()
    {
        _identity.Setup(i => i.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new IdentityCreateResult(false, null, ["Email already taken."]));

        var act = () => _sut.RegisterAsync(new RegisterRequest("taken@watchlog.test", "Password1", "Someone"));

        await act.Should().ThrowAsync<ConflictException>().WithMessage("*already taken*");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ThrowsConflictException()
    {
        _identity.Setup(i => i.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Guid?)null);

        var act = () => _sut.LoginAsync(new LoginRequest("nobody@watchlog.test", "wrong-password"));

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_IssuesTokens()
    {
        var userId = Guid.NewGuid();
        _identity.Setup(i => i.ValidateCredentialsAsync("user@watchlog.test", "correct-password")).ReturnsAsync(userId);
        _identity.Setup(i => i.GetRolesAsync(userId)).ReturnsAsync((IReadOnlyList<string>)[]);

        var result = await _sut.LoginAsync(new LoginRequest("user@watchlog.test", "correct-password"));

        result.UserId.Should().Be(userId);
        result.AccessToken.Should().Be("fake-access-token");
    }

    [Fact]
    public async Task RefreshAsync_WithExpiredToken_ThrowsForbiddenException()
    {
        var userId = Guid.NewGuid();
        _tokens.Setup(t => t.HashRefreshToken("expired-token")).Returns("expired-hash");
        _unitOfWork.Seed(new RefreshToken
        {
            UserId = userId,
            TokenHash = "expired-hash",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1) // already expired
        });

        var act = () => _sut.RefreshAsync(new RefreshRequest("expired-token"));

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
