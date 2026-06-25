namespace ClaimsModule.Domain.Exceptions;

public sealed class ConcurrencyException(string entityName, object key)
    : Exception($"{entityName} with key '{key}' was modified by another user. Please refresh and try again.");