using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Areas.Identity.Data;
using EBookShopR.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Models
{
    public class IdentityDBContext : IdentityDbContext<ApplicationUser,ApplicationRole,string,IdentityUserClaim<string>, ApplictionUserRole,IdentityUserLogin<string>,IdentityRoleClaim<string>,IdentityUserToken<string>>
    {
        public IdentityDBContext(DbContextOptions<IdentityDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationRole>().ToTable("AspNetRoles").ToTable("AppRoles");
            builder.Entity<ApplictionUserRole>().ToTable("AppUserRole");
            builder.Entity<ApplictionUserRole>()
                .HasOne(userRole => userRole.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(f => f.RoleId);
            builder.Entity<ApplicationUser>().ToTable("AppUsers");

            builder.Entity<ApplictionUserRole>()
                .HasOne(UserRole => UserRole.User)
                .WithMany(User => User.Roles).HasForeignKey(f => f.UserId);
        }
    }
}
