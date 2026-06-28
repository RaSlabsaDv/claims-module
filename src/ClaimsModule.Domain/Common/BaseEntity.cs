namespace ClaimsModule.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public Guid? UserCreated { get; private set; }
    public Guid? UserModified { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    protected BaseEntity() { }

    protected void SetCreated(Guid userId)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        UserCreated = userId;
    }

    protected void SetUpdated(Guid userId)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UserModified = userId;
    }

    public void SoftDelete(Guid userId)
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        SetUpdated(userId);
    }

    protected void SetId(Guid id) => Id = id;
}