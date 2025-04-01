using FinanceApp.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FinanceApp.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID was not found in token.");
        }
        UserId = Guid.Parse(userIdClaim);
    }
}
