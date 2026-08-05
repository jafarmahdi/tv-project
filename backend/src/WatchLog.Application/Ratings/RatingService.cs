using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Ratings;

public class RatingService(IUnitOfWork unitOfWork) : IRatingService
{
    public async Task RateAsync(Guid userId, RateRequest request, CancellationToken ct = default)
    {
        if (request.Score is < 1 or > 10)
        {
            throw new ConflictException("Score must be between 1 and 10.");
        }

        var repo = unitOfWork.Repository<Rating>();
        var existing = await repo.Query()
            .FirstOrDefaultAsync(r => r.UserId == userId && r.TargetType == request.TargetType && r.TargetId == request.TargetId, ct);

        if (existing is null)
        {
            await repo.AddAsync(new Rating { UserId = userId, TargetType = request.TargetType, TargetId = request.TargetId, Score = request.Score }, ct);
        }
        else
        {
            existing.Score = request.Score;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
            repo.Update(existing);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<RatingSummaryDto> GetSummaryAsync(Guid? userId, TargetType targetType, Guid targetId, CancellationToken ct = default)
    {
        var ratings = await unitOfWork.Repository<Rating>().Query()
            .Where(r => r.TargetType == targetType && r.TargetId == targetId)
            .ToListAsync(ct);

        var mine = userId is null ? null : ratings.FirstOrDefault(r => r.UserId == userId)?.Score;
        return new RatingSummaryDto(ratings.Count == 0 ? 0 : ratings.Average(r => r.Score), ratings.Count, mine);
    }
}
