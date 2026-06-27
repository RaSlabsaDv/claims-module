namespace ClaimsModule.API.Controllers.Claims;

public sealed record UploadDocumentRequest(
    byte[] RowVersion,
    string DocumentType,
    string DocumentName,
    IFormFile File);