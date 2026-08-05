using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WatchLog.Application.Auth;

namespace WatchLog.Api.IntegrationTests;

public class AuthEndpointsTests(WatchLogApiFactory factory) : IClassFixture<WatchLogApiFactory>, IAsyncLifetime
{
    private HttpClient _client = default!;

    public async Task InitializeAsync()
    {
        await factory.MigrateAsync();
        _client = factory.CreateClient();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_WithValidPayload_Returns200AndAccessToken()
    {
        var request = new RegisterRequest($"user-{Guid.NewGuid():N}@watchlog.test", "Password1", "Integration Test User");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        result!.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_WithWeakPassword_Returns400WithValidationErrors()
    {
        var request = new RegisterRequest($"user-{Guid.NewGuid():N}@watchlog.test", "weak", "Someone");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns409()
    {
        var email = $"user-{Guid.NewGuid():N}@watchlog.test";
        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, "Password1", "Someone"));

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, "WrongPassword1"));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_Returns200()
    {
        var email = $"user-{Guid.NewGuid():N}@watchlog.test";
        var register = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, "Password1", "Someone"));
        var auth = await register.Content.ReadFromJsonAsync<AuthResult>();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/users/me");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
