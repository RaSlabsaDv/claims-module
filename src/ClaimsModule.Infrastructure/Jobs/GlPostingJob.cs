using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Enums;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace ClaimsModule.Infrastructure.Jobs;

public sealed class GlPostingJob(
    IReserveRepository reserveRepository,
    IUnitOfWork unitOfWork,
    IAuditLogService auditLog,
    ILogger<GlPostingJob> logger,
    PerformContext context)
{
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync(Guid reserveHistoryId, string idempotencyKey)
    {
        var transaction = await reserveRepository.GetTransactionByIdAsync(reserveHistoryId);

        if (transaction is null)
        {
            logger.LogWarning("GL Posting: ReserveHistory {Id} not found. Skipping.", reserveHistoryId);
            return;
        }

        // Idempotency check — ТЗ Section 12.1
        if (transaction.PostingStatus == GlPostingStatus.Posted)
        {
            logger.LogInformation("GL Posting: {Key} already posted. Skipping.", idempotencyKey);
            return;
        }

        var jobId = context.BackgroundJob.Id;

        try
        {
            var journalEntry = new
            {
                DR = "Change in Outstanding Reserves",
                CR = "Outstanding Loss Reserves",
                Amount = transaction.Amount,
                Currency = "USD",
                PostedAt = DateTimeOffset.UtcNow
            };

            transaction.MarkPosted(jobId);
            await unitOfWork.SaveChangesAsync();

            await auditLog.LogAsync(
                claimId: transaction.ClaimId,
                eventType: AuditEventTypes.GlPostingSimulated,
                description: $"GL posting simulated for reserve transaction {reserveHistoryId}.",
                relatedEntityId: reserveHistoryId,
                relatedEntityType: nameof(Domain.Entities.ReserveHistory),
                newValue: journalEntry);

            logger.LogInformation("GL Posting: successfully posted for {Id}.", reserveHistoryId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GL Posting: failed for {Id}.", reserveHistoryId);

            transaction.MarkPostingFailed();
            await unitOfWork.SaveChangesAsync();

            await auditLog.LogAsync(
                claimId: transaction.ClaimId,
                eventType: AuditEventTypes.GlPostingFailed,
                description: $"GL posting failed for reserve transaction {reserveHistoryId}: {ex.Message}",
                relatedEntityId: reserveHistoryId,
                relatedEntityType: nameof(Domain.Entities.ReserveHistory),
                newValue: new { Reason = ex.Message });

            throw;
        }
    }
}
