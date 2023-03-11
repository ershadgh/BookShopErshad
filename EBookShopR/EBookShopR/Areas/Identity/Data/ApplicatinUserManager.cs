using BookShop.Areas.Identity.Data;
using EBookShopR.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EBookShopR.Areas.Identity.Data
{
    
        public class ApplicationUserManager : UserManager<ApplicationUser>, IApplicationUserManager
    {
            private readonly ApplicationIdentityErrorDescriber _error;
            private readonly ILookupNormalizer _keynormalizer;
            private readonly ILogger<ApplicationUserManager> _logger;
            private readonly IOptions<IdentityOptions> _options;
            private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
            private readonly IEnumerable<IPasswordValidator<ApplicationUser>> _passwordValidators;
            private readonly IServiceProvider _services;
            private readonly IUserStore<ApplicationUser> _userStore;
            private readonly IEnumerable<IUserValidator<ApplicationUser>> _userValidators;

            public ApplicationUserManager(IUserStore<ApplicationUser> store,
                IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher,
                IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
                ILookupNormalizer keyNormalizer, ApplicationIdentityErrorDescriber errors,
                IServiceProvider services, ILogger<ApplicationUserManager> logger) : base(store, optionsAccessor,
                    passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
            {
                _services = services;
                _userStore = store;
                _passwordHasher = passwordHasher;
                _userValidators = userValidators;
                _passwordValidators = passwordValidators;
                _keynormalizer = keyNormalizer;
                _error = errors;
                _logger = logger;
                _options = optionsAccessor;

            }

       

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
            {
                return await Users.ToListAsync();
            }
        public async Task<List<UsersViewModel>> GetAllUserWhitRoleAsync()
        {
            return await Users.Select(user => new UsersViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber=user.PhoneNumber,
                FirstName=user.FristName,
                LastName=user.LastName,
                BirthDate=user.BirthDate,
                IsActive=user.IsActive,
                LastVisitDateTime=user.LastVisitDateTime,
                Image=user.Image,
                RegisterDate=user.RegisterDate,
                Roles=user.Roles.Select(u=>u.Role.Name),
            }).ToListAsync();
        }

        public string NormalizeKey(string key)
        {
            throw new NotImplementedException();
        }
        public async Task<UsersViewModel> FindUserWithRoleByIdAsync(string UserId)
        {
            return await Users.Where(u => u.Id == UserId).Select(user => new UsersViewModel
            {

                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FristName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                IsActive = user.IsActive,
                LastVisitDateTime = user.LastVisitDateTime,
                Image = user.Image,
                RegisterDate = user.RegisterDate,
                Roles = user.Roles.Select(u => u.Role.Name),
                AccessFailedCount = user.AccessFailedCount, 
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled
            }).FirstOrDefaultAsync();
        }

        public async Task<string> GetFullName(ClaimsPrincipal User)
        {
            var UserInfo=await GetUserAsync(User);
            return UserInfo.FristName + "" + UserInfo.LastName;
        }
    }
}
