using ClaimsModule.Application.Common.Interfaces;
using System.Security.Claims;

namespace ClaimsModule.API.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private HttpContext? Context => httpContextAccessor.HttpContext;

    public Guid? UserId
    {
        get
        {
            var value = Context?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var guid) ? guid : null;
        }
    }

    public string? Role => Context?.User.FindFirstValue(ClaimTypes.Role);

    public Guid? OrganisationId
    {
        get
        {
            var value = Context?.User.FindFirstValue("organisation_id");
            return Guid.TryParse(value, out var guid) ? guid : null;
        }
    }

    public Guid? CorrelationId
    {
        get
        {
            var value = Context?.Items["CorrelationId"]?.ToString();
            return Guid.TryParse(value, out var guid) ? guid : null;
        }
    }
}