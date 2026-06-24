namespace ClaimsModule.Application.Common.Interfaces;

public interface IClaimNumberGenerator
{
    /// <summary>
    /// Атомарно генерує наступний номер позову у форматі CLM-{YYYY}-{7digits}.
    /// Використовує UPDATE...OUTPUT INSERTED на ClaimSequences щоб уникнути race condition.
    /// </summary>
    Task<string> GenerateAsync(Guid organisationId, CancellationToken ct = default);
}
