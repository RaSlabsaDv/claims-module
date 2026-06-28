namespace ClaimsModule.API.Controllers.Claims;

public sealed record UploadDocumentRequest(
    string RowVersion,
    string DocumentType,
    string DocumentName,
    IFormFile File);