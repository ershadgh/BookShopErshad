﻿using System.ComponentModel.DataAnnotations;

namespace EBookShopR.Models.ViewModels
{
    public class UsersViewModel
    {
        public string Id { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "شماره موبایل")]
        public string PhoneNumber { get; set; }

        [Display(Name = "نام")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string? LastName { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }
        [Display(Name = "تاریخ تولد")]
        public string PersianBirthDate { get; set; }

        [Display(Name = "تصویر پروفایل")]
        public string Image { get; set; }

        [Display(Name = "تاریخ عضویت")]
        public DateTime? RegisterDate { get; set; }

        [Display(Name = "آخرین بازدید")]
        public DateTime? LastVisitDateTime { get; set; }

        [Display(Name = "فعال / غیرفعال")]
        public bool? IsActive { get; set; }

        [Display(Name = "نقش ها")]
        public IEnumerable<string> Roles { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public bool LockoutEnabled { get; set; }

        public bool EmailConfirmed { get; set; }

        public int AccessFailedCount { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }
    }
    public class UserResetPasswordViewModel
    {
        public string Id { get; set; }
        [Display(Name ="نام کاربری")]
        public string? UserName { get; set; }
        [Display(Name ="ایمیل")]
        public string? Email { get; set; }
        [Required(ErrorMessage ="وارد نمودن {0} الزامی می باشد.")]
        [Display(Name ="کلمه عبور جدید")]
        public string NewPassword { get; set; }
    }
}
