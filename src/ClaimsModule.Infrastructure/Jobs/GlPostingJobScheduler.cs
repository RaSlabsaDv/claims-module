using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Infrastructure.Jobs;
using Hangfire;

namespace ClaimsModule.Infrastructure.Services;

public sealed class GlPostingJobScheduler : IGlPostingJobScheduler
{
    public void EnqueueGlPosting(Guid reserveHistoryId, string idempotencyKey)
    {
        BackgroundJob.Enqueue<GlPostingJob>(
            job => job.ExecuteAsync(reserveHistoryId, idempotencyKey, null!));
    }
}