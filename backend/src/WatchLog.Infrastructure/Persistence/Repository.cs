using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Common;

namespace WatchLog.Infrastructure.Persistence;

public class Repository<TEntity>(WatchLogDbContext dbContext) : IRepository<TEntity> where TEntity : Entity
{
    private readonly DbSet<TEntity> _set = dbContext.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default) => await _set.FindAsync([id], ct);

    public IQueryable<TEntity> Query() => _set.AsQueryable();

    public async Task AddAsync(TEntity entity, CancellationToken ct = default) => await _set.AddAsync(entity, ct);

    public void Update(TEntity entity) => _set.Update(entity);

    public void Remove(TEntity entity) => _set.Remove(entity);
}
