namespace ClaimsModule.Domain.Exceptions;

/// <summary>
/// Кидається коли запитувана сутність не знайдена.
/// Mapped to HTTP 404 Not Found на рівні API.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}
