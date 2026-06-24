namespace ClaimsModule.Application.Common.Interfaces;

public interface IGlPostingJobScheduler
{
    /// <summary>
    /// Ставить у чергу Hangfire job для симуляції GL проводки.
    /// Ключ ідемпотентності: Reserve:{reserveId}:Change:{changeSequence}
    /// </summary>
    void EnqueueGlPosting(Guid reserveHistoryId, string idempotencyKey);
}
