using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using ClaimsModule.Application.Common.Interfaces;

namespace ClaimsModule.Infrastructure.Services;

public sealed class AzureBlobStorageService(
    BlobServiceClient blobServiceClient,
    string containerName) : IStorageService
{
    public async Task<string> UploadAsync(
        string blobPath,
        Stream content,
        string contentType,
        CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blobClient = containerClient.GetBlobClient(blobPath);
        await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return blobPath;
    }

    public async Task<string> GetDownloadUrlAsync(string blobPath, CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));

        return await Task.FromResult(sasUri.ToString());
    }

    public async Task DeleteAsync(string blobPath, CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }
}