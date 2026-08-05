using FluentAssertions;
using Moq;
using WatchLog.Application.Catalog;
using WatchLog.Application.Common;
using WatchLog.Application.Lists;
using WatchLog.Application.Tests.TestSupport;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Tests.Lists;

public class ListServiceTests
{
    private readonly Mock<ICatalogService> _catalog = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly ListService _sut;

    public ListServiceTests()
    {
        _sut = new ListService(_catalog.Object, _unitOfWork);
    }

    [Fact]
    public async Task CreateCustomListAsync_CreatesListWithCustomType()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CreateCustomListAsync(userId, new CreateCustomListRequest("Comfort Rewatches"));

        result.Name.Should().Be("Comfort Rewatches");
        result.Type.Should().Be(ListType.Custom);
        _unitOfWork.Repository<UserList>().Query().Should().ContainSingle(l => l.UserId == userId && l.Name == "Comfort Rewatches");
    }

    [Fact]
    public async Task AddItemAsync_WhenListBelongsToAnotherUser_ThrowsForbiddenException()
    {
        var ownerId = Guid.NewGuid();
        var attackerId = Guid.NewGuid();
        var list = new UserList { UserId = ownerId, Name = "Watching", Type = ListType.Watching };
        _unitOfWork.Seed(list);

        var act = () => _sut.AddItemAsync(attackerId, list.Id, new AddListItemRequest(MovieTmdbId: 550));

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task AddItemAsync_WhenListDoesNotExist_ThrowsNotFoundException()
    {
        var act = () => _sut.AddItemAsync(Guid.NewGuid(), Guid.NewGuid(), new AddListItemRequest(MovieTmdbId: 550));

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddItemAsync_WithMovie_ResolvesLocalMovieIdViaCatalog()
    {
        var userId = Guid.NewGuid();
        var list = new UserList { UserId = userId, Name = "Watching", Type = ListType.Watching };
        _unitOfWork.Seed(list);

        var localMovieId = Guid.NewGuid();
        _catalog.Setup(c => c.EnsureMovieCachedAsync(550, It.IsAny<CancellationToken>())).ReturnsAsync(localMovieId);

        await _sut.AddItemAsync(userId, list.Id, new AddListItemRequest(MovieTmdbId: 550));

        _unitOfWork.Repository<ListItem>().Query().Should().ContainSingle(i => i.ListId == list.Id && i.MovieId == localMovieId);
    }

    [Fact]
    public async Task DeleteCustomListAsync_WhenListIsBuiltIn_ThrowsConflictException()
    {
        var userId = Guid.NewGuid();
        var list = new UserList { UserId = userId, Name = "Watching", Type = ListType.Watching };
        _unitOfWork.Seed(list);

        var act = () => _sut.DeleteCustomListAsync(userId, list.Id);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task GetUserListsAsync_ReturnsOnlyTheCallersLists()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        _unitOfWork.Seed(
            new UserList { UserId = userId, Name = "Watching", Type = ListType.Watching },
            new UserList { UserId = userId, Name = "Favorites", Type = ListType.Favorites },
            new UserList { UserId = otherUserId, Name = "Watching", Type = ListType.Watching });

        var result = await _sut.GetUserListsAsync(userId);

        result.Should().HaveCount(2);
    }
}
