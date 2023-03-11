using System.Security.Claims;

namespace EBookShopR.Areas.Admin.Services
{
    public interface ISecurityTrimingService
    {
        bool CanCurrentUserAccess(string area, string controller, string action);
        bool CanUserAccess(ClaimsPrincipal user, string area, string controller, string action);
    }
}
