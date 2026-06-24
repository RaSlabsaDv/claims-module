using ClaimsModule.Application.Common.Interfaces;

namespace ClaimsModule.Infrastructure.Services;

public sealed class LocalFileSystemStorageService(
    string basePath,
    string baseUrl) : IStorageService
{
    public async Task<string> UploadAsync(
        string blobPath,
        Stream content,
        string contentType,
        CancellationToken ct = default)
    {
        var fullPath = Path.Combine(basePath, blobPath.Replace('/', Path.DirectorySeparatorChar));
        var directory = Path.GetDirectoryName(fullPath)!;

        Directory.CreateDirectory(directory);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, ct);

        return blobPath;
    }

    public Task<string> GetDownloadUrlAsync(string blobPath, CancellationToken ct = default)
    {
        var url = $"{baseUrl.TrimEnd('/')}/uploads/{blobPath.Replace(Path.DirectorySeparatorChar, '/')}";
        return Task.FromResult(url);
    }

    public Task DeleteAsync(string blobPath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(basePath, blobPath.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}