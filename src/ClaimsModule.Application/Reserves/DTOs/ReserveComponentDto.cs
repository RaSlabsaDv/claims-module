using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Reserves.Dtos;

public sealed record ReserveComponentDto(
    Guid Id,
    Guid ClaimId,
    ReserveComponentType ComponentType,
    decimal CurrentAmount,
    ReserveComponentStatus Status,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyList<ReserveHistoryDto> Transactions
);