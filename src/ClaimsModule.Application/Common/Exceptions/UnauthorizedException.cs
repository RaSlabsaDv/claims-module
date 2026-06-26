namespace ClaimsModule.Application.Common.Exceptions;

public sealed class UnauthorizedException()
    : Exception("User is not authenticated.");