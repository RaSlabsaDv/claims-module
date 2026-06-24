namespace ClaimsModule.Domain.Entities;

// Reference / lookup — без BaseEntity (немає soft delete, audit columns)
public sealed class CauseOfLossCode
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string PerilCategory { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }

    // EF Core
    private CauseOfLossCode() { }
}
