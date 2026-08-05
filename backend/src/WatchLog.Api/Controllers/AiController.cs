using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Ai;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Controllers;

[Authorize]
[Route("api/v1/ai/assistant")]
public class AiController(IAiAssistantService aiService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpPost("ask")]
    public async Task<ActionResult<AiResponseDto>> Ask(AiAskRequest request, CancellationToken ct) =>
        Ok(await aiService.AskAsync(CurrentUserId, request.Prompt, ct));

    [HttpGet("history")]
    public async Task<ActionResult<IReadOnlyList<AiHistoryItemDto>>> GetHistory(CancellationToken ct) =>
        Ok(await aiService.GetHistoryAsync(CurrentUserId, ct));
}
