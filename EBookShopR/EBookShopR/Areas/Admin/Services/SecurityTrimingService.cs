using EBookShopR.Areas.Admin.Data;
using System.Security.Claims;

namespace EBookShopR.Areas.Admin.Services
{
    public class SecurityTrimingService: ISecurityTrimingService
    {
        private readonly HttpContext _httpContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMvcActionsDiscoveryService _mvcActionsDiscoveryService;
        public SecurityTrimingService(HttpContext httpContext,IHttpContextAccessor contextAccessor,IMvcActionsDiscoveryService mvcActionsDiscoveryService)
        {
            _httpContext=httpContext;
            _contextAccessor=contextAccessor;
            _mvcActionsDiscoveryService=mvcActionsDiscoveryService;
        }
        public bool CanCurrentUserAccess(string area,string controler,string action)
        {
            return _httpContext != null;
        }
        public bool CanUserAccess(ClaimsPrincipal user,string area,string controller,string action)
        {
            var currentClaimValue = $"{area}:{controller}:{action}";
            var securedControllerAction = _mvcActionsDiscoveryService.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies.DynamicPermission);
            if(!securedControllerAction.SelectMany(x=>x.MvcAction).Any(x=>x.ActionId==currentClaimValue))
            {
                throw new KeyNotFoundException($@"The `secured` area={area}/controller={controller}/action={action} with `ConstantPolicies.DynamicPermission` policy not found. Please check you have entered the area/controller/action names correctly and also it's decorated with the correct security policy.");
            }
            if(!user.Identity.IsAuthenticated)
            {
                return false;
            }
            return user.HasClaim(claim => claim.Type == ConstantPolicies.DynamicPermissionClaimType &&
                                         claim.Value == currentClaimValue);
        }
    }
}
