using WatchLog.Domain.Common;

namespace WatchLog.Application.Common.Interfaces;

/// <summary>
/// Generic repository abstraction over an aggregate. Application services depend only on this
/// (and the specialized repositories below) — never on EF Core or `DbContext` directly, which is
/// what lets `WatchLog.Application` stay persistence-agnostic and easily unit-testable with mocks.
/// </summary>
public interface IRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Escape hatch for read queries that need filtering/paging/includes beyond a simple lookup.</summary>
    IQueryable<TEntity> Query();

    Task AddAsync(TEntity entity, CancellationToken ct = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

/// <summary>Coordinates one or more repositories under a single EF Core `SaveChanges` transaction.</summary>
public interface IUnitOfWork
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
