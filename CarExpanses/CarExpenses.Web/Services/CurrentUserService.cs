using System.Security.Claims;
using CarExpenses.Model.Security;
using Microsoft.AspNetCore.Http;

namespace CarExpenses.Web.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var principal = httpContextAccessor.HttpContext?.User;
            if (principal is null)
            {
                return null;
            }

            var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idValue, out var id) ? id : null;
        }
    }

    public bool IsInRole(string role)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        return principal?.IsInRole(role) ?? false;
    }
}
