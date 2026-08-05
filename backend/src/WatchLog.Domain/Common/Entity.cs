namespace WatchLog.Domain.Common;

/// <summary>
/// Base class for every domain entity. Deliberately minimal — no EF Core or
/// ASP.NET dependencies belong in the Domain layer.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

/// <summary>
/// Adds creation/update auditing to an entity.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}
