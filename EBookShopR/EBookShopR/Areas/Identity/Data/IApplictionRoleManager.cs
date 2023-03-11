using EBookShopR.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace EBookShopR.Areas.Identity.Data
{
    public interface IApplictionRoleManager
    {
        #region BaseClass
        IQueryable<ApplicationRole> Roles { get; }
        ILookupNormalizer KeyNormalizer { get; set; }
        IdentityErrorDescriber ErrorDescriber { get; set; }
        IList<IRoleValidator<ApplicationRole>> RoleValidators { get; }
        bool SupportsQueryableRoles { get; }
        bool SupportsRoleClaims { get; }
        Task<IdentityResult> CreateAsync(ApplicationRole role);
        Task<IdentityResult> DeleteAsync(ApplicationRole role);
        Task<ApplicationRole> FindByIdAsync(string roleId);
        Task<ApplicationRole> FindByNameAsync(string roleName);
        string NormalizeKey(string key);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> UpdateAsync(ApplicationRole role);
        Task UpdateNormalizedRoleNameAsync(ApplicationRole role);
        Task<string> GetRoleNameAsync(ApplicationRole role);
        Task<IdentityResult> SetRoleNameAsync(ApplicationRole role, string name);
        #endregion
        #region CustomMethod
        List<ApplicationRole> GetAllRoles();
        List<RolesViewModel> GetAllRolesAndUsersCount();


        #endregion
    }
}
