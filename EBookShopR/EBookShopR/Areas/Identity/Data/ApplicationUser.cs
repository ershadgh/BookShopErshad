﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EBookShopR.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the BookShopUser class
    public class ApplicationUser : IdentityUser
    {
        public String? FristName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Image { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? LastVisitDateTime { get; set; }
        public bool? IsActive { get; set; }
        public virtual List<ApplictionUserRole> Roles{ get; set; }
    }
}
