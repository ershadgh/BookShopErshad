using EBookShopR.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace EBookShopR.Areas.Identity.Data
{
    public class ApplictionRoleManager:RoleManager<ApplicationRole>, IApplictionRoleManager
    {
        private readonly IRoleStore<ApplicationRole> _Store;
        private readonly IEnumerable<IRoleValidator<ApplicationRole>> _roleValidators;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly IdentityErrorDescriber _errors;
        private readonly ILogger<ApplictionRoleManager> _logger;
      
        public ApplictionRoleManager(
            IRoleStore<ApplicationRole> store,
            IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<ApplictionRoleManager> logger):base(store,roleValidators,keyNormalizer,errors,logger)
        {
            _errors = errors;
            _keyNormalizer=keyNormalizer;
            _logger = logger;
            _roleValidators=roleValidators;
            _Store=store;

        }
        public  List<ApplicationRole> GetAllRoles()
        {
            return Roles.ToList();
        }
        public List<RolesViewModel> GetAllRolesAndUsersCount()
        {
            return Roles.Select(role =>
            new RolesViewModel
            {
                RoleID = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                UserCount=role.Users.Count(),
               
            }).ToList();
        }
    }
}
