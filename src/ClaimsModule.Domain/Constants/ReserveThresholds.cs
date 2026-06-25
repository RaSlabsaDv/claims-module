namespace ClaimsModule.Domain.Constants;

public static class ReserveThresholds
{
    public const decimal AutoApprovalLimit = 10_000m;
    public const decimal SupervisorLimit = 100_000m;
    public const decimal ManagerLimit = 10_000_000m;
}