using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Collections;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Controllers;

public class CollectionsController(ICollectionService collectionService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [AllowAnonymous]
    [HttpGet("curated")]
    public async Task<ActionResult<IReadOnlyList<CollectionSummaryDto>>> GetCurated(CancellationToken ct) =>
        Ok(await collectionService.GetCuratedAsync(ct));

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<CollectionSummaryDto>>> GetMine(CancellationToken ct) =>
        Ok(await collectionService.GetUserCollectionsAsync(CurrentUserId, ct));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CollectionDetailDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await collectionService.GetByIdAsync(id, ct));

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CollectionSummaryDto>> Create(CreateCollectionRequest request, CancellationToken ct) =>
        Ok(await collectionService.CreateAsync(CurrentUserId, request, ct));

    [Authorize]
    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, AddCollectionItemRequest request, CancellationToken ct)
    {
        await collectionService.AddItemAsync(CurrentUserId, id, request, ct);
        return NoContent();
    }
}
