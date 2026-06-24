using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ClaimsModule.Infrastructure.Jobs;

public sealed class SlaMonitoringJob(
    IClaimRepository claimRepository,
    IClaimAuditLogRepository auditLogRepository,
    IAuditLogService auditLog,
    ILogger<SlaMonitoringJob> logger)
{
    private static readonly ClaimStatus[] MonitoredStatuses =
        [ClaimStatus.Draft, ClaimStatus.Open];

    public async Task ExecuteAsync()
    {
        var slaThreshold = DateTimeOffset.UtcNow.AddHours(-48);
        var deduplicationWindow = DateTimeOffset.UtcNow.AddHours(-24);

        var filter = new ClaimListFilter(Status: null);

        // Отримуємо всі stale claims через репозиторій
        var (staleClaims, _) = await claimRepository.ListAsync(
            new ClaimListFilter(),
            page: 1,
            pageSize: int.MaxValue);

        // Note: Loading all stale claims at once. Acceptable for assessment scope.
        // Production implementation should use batching or a dedicated query.
        var staleClaimIds = staleClaims
            .Where(c => MonitoredStatuses.Contains(c.Status)
                     && c.UpdatedAt < slaThreshold)
            .Select(c => c.Id)
            .ToList();

        if (!staleClaimIds.Any())
        {
            logger.LogInformation("SLA Monitor: no stale claims found.");
            return;
        }

        foreach (var claimId in staleClaimIds)
        {
            // Idempotency — перевіряємо останній SLA_BREACH_DETECTED за 24 год
            var lastBreach = await auditLogRepository.GetLastByEventTypeAsync(
                claimId,
                AuditEventTypes.SlaBreachDetected);

            if (lastBreach is not null && lastBreach.CreatedAt >= deduplicationWindow)
            {
                logger.LogInformation("SLA Monitor: claim {ClaimId} already flagged. Skipping.", claimId);
                continue;
            }

            await auditLog.LogAsync(
                claimId: claimId,
                eventType: AuditEventTypes.SlaBreachDetected,
                description: "Claim has not been updated in 48 hours.");

            logger.LogWarning("SLA Monitor: breach detected for claim {ClaimId}.", claimId);
        }
    }
}
