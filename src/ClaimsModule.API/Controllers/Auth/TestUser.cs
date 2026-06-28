namespace ClaimsModule.API.Controllers.Auth;

public sealed class TestUser
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string Role { get; init; } = default!;
    public string UserId { get; init; } = default!;
    public string OrganisationId { get; init; } = default!;
}