namespace ClaimsModule.Domain.Exceptions;

/// <summary>
/// Кидається коли порушено бізнес-правило домену.
/// Mapped to HTTP 422 Unprocessable Entity на рівні API.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message)
        : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
