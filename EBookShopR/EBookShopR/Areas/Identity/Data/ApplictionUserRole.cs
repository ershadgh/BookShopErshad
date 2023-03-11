using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace EBookShopR.Areas.Identity.Data
{
    public class ApplictionUserRole:IdentityUserRole<string>
    {
        public virtual ApplicationRole Role { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
