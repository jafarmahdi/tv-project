using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using WatchLog.Application.Auth;
using WatchLog.Application.Lists;

namespace WatchLog.Api.IntegrationTests;

public class ListsEndpointsTests(WatchLogApiFactory factory) : IClassFixture<WatchLogApiFactory>, IAsyncLifetime
{
    private HttpClient _client = default!;

    public async Task InitializeAsync()
    {
        await factory.MigrateAsync();
        _client = factory.CreateClient();

        var email = $"user-{Guid.NewGuid():N}@watchlog.test";
        var register = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, "Password1", "Lists Test User"));
        var auth = await register.Content.ReadFromJsonAsync<AuthResult>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task NewUser_AlreadyHasSixBuiltInLists()
    {
        var response = await _client.GetAsync("/api/v1/lists");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lists = await response.Content.ReadFromJsonAsync<List<UserListDto>>();
        lists.Should().HaveCount(6);
    }

    [Fact]
    public async Task CreateCustomList_ThenAppearsInMyLists()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/lists", new CreateCustomListRequest("Cozy Winter Rewatches"));
        create.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await _client.GetAsync("/api/v1/lists");
        var lists = await response.Content.ReadFromJsonAsync<List<UserListDto>>();

        lists.Should().Contain(l => l.Name == "Cozy Winter Rewatches" && l.Type == Domain.Enums.ListType.Custom);
    }

    [Fact]
    public async Task DeleteBuiltInList_Returns409Conflict()
    {
        var listsResponse = await _client.GetAsync("/api/v1/lists");
        var lists = await listsResponse.Content.ReadFromJsonAsync<List<UserListDto>>();
        var builtIn = lists!.First(l => l.Type == Domain.Enums.ListType.Watching);

        var response = await _client.DeleteAsync($"/api/v1/lists/{builtIn.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
