using ClaimsModule.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClaimsModule.Infrastructure.Services;

public sealed class MockGlPostingJobScheduler(
    ILogger<MockGlPostingJobScheduler> logger) : IGlPostingJobScheduler
{
    public void EnqueueGlPosting(Guid reserveHistoryId, string idempotencyKey)
        => logger.LogInformation(
            "Mock GL Posting enqueued for {ReserveHistoryId} with key {IdempotencyKey}",
            reserveHistoryId,
            idempotencyKey);
}