using System.Collections.Concurrent;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Common;

namespace WatchLog.Infrastructure.Persistence;

public class UnitOfWork(WatchLogDbContext dbContext) : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity =>
        (IRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity), _ => new Repository<TEntity>(dbContext));

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => dbContext.SaveChangesAsync(ct);
}
