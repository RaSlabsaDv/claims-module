namespace ClaimsModule.Application.Common.Interfaces;

public interface IStorageService
{
    /// <summary>
    /// Завантажує файл у сховище та повертає blob path.
    /// </summary>
    Task<string> UploadAsync(
        string blobPath,
        Stream content,
        string contentType,
        CancellationToken ct = default);

    /// <summary>
    /// Повертає short-lived URL для завантаження (SAS URL 1h TTL або local URL).
    /// </summary>
    Task<string> GetDownloadUrlAsync(string blobPath, CancellationToken ct = default);

    Task DeleteAsync(string blobPath, CancellationToken ct = default);
}
