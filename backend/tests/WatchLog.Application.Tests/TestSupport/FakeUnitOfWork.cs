using MockQueryable.Moq;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Common;

namespace WatchLog.Application.Tests.TestSupport;

/// <summary>
/// A real (non-Moq) in-memory `IUnitOfWork`/`IRepository&lt;T&gt;` test double. Backed by plain
/// `List&lt;T&gt;`s, with `Query()` wrapped via MockQueryable so EF Core's async LINQ extensions
/// (`FirstOrDefaultAsync`, `ToListAsync`, ...) — which the real repositories rely on — work exactly
/// as they would against a real `DbSet&lt;T&gt;`.
/// </summary>
public class FakeUnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();
    public int SaveChangesCallCount { get; private set; }

    public List<T> Seed<T>(params T[] entities) where T : Entity
    {
        var repo = (FakeRepository<T>)Repository<T>();
        repo.Items.AddRange(entities);
        return repo.Items;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
    {
        if (!_repositories.TryGetValue(typeof(TEntity), out var repo))
        {
            repo = new FakeRepository<TEntity>();
            _repositories[typeof(TEntity)] = repo;
        }
        return (IRepository<TEntity>)repo;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        SaveChangesCallCount++;
        return Task.FromResult(1);
    }

    private class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        public List<TEntity> Items { get; } = [];

        public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

        public IQueryable<TEntity> Query() => Items.AsQueryable().BuildMockDbSet().Object;

        public Task AddAsync(TEntity entity, CancellationToken ct = default)
        {
            Items.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(TEntity entity)
        {
            var index = Items.FindIndex(x => x.Id == entity.Id);
            if (index >= 0) Items[index] = entity;
        }

        public void Remove(TEntity entity) => Items.RemoveAll(x => x.Id == entity.Id);
    }
}
