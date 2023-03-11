using System.ComponentModel.DataAnnotations;

namespace EBookShopR.Models.ViewModels
{
    public class RolesViewModel
    {
        public string? RoleID { get; set; }
        [Display(Name ="عنوان نقش")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
        
        public string? RoleName { get; set; }
        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string?  Description { get; set; }
        [Display(Name ="کاربران")]
        public int? UserCount { get; set; }
       
    }
}
