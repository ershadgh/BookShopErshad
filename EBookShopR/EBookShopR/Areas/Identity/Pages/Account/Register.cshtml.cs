using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using EBookShopR.Areas.Identity.Data;
using BookShop.Classes;

namespace BookShop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        // private readonly IEmailSender _emailSender;
        private readonly IApplictionRoleManager _roleManager;

        public RegisterModel( 
            UserManager<ApplicationUser> userManager,
           // SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IApplictionRoleManager roleManager
           // IEmailSender emailSender
           )
        {
            _userManager = userManager;
           // _signInManager = signInManager;
            _logger = logger;
            //  _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        
        public string ReturnUrl { get; set; }
        public IEnumerable<ApplicationRole> GetRoles { get; set; }
        [BindProperty]
        public string[] UserRoles { get; set; }
        public class InputModel
        {
          
            [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
            [EmailAddress(ErrorMessage ="ایمیل شما نامعتبر است")]
            [Display(Name = "ایمیل")]
            public string Email { get; set; }

            [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
            [StringLength(100, ErrorMessage = " {0}باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "کلمه عبور")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "تکرار کلمه عبور")]
            [Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
            public string ConfirmPassword { get; set; }
            [Display(Name ="نام")]
            [Required(ErrorMessage =" وارد نمودن {0} الزامی است")]
            public string Name { get; set; }
            [Display(Name ="نام خانوادگی")]
            [Required(ErrorMessage =" وارد نمودن {0} الزامی است")]
            public string Family { get; set; }
            [Display(Name ="تاریخ تولد")]
            [Required(ErrorMessage =" وارد نمودن {0}  الزامی است")]
            public string BirthDate { get; set; }
            [Display(Name ="نام کاربری")]
            [Required(ErrorMessage ="وارد نمودن {0} الزامی است")]
            public string UserName { get; set; }
            [Display(Name ="شماره موبایل")]
            [Required(ErrorMessage ="وارد نمودن {0} الزامی است")]
            public string PhoneNumber { get; set; }
            [Display(Name ="احراز هویت دو مرحله ی")]
            public bool TwoFactorEnabled { get; set; }

        }

        public void OnGet(string? returnUrl = null)
        {
            GetRoles = _roleManager.GetAllRoles();
            ReturnUrl = returnUrl;
            
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/UserManager/Index?Msg=Success");
            if (ModelState.IsValid)
            {
                ConvertDate convert = new ConvertDate();
                //  var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email,FristName=Input.Name,LastName=Input.Family,PhoneNumber=Input.PhoneNumber,BirthDate=convert.ConvertShamsiToMiladi(Input.BirthDate),IsActive=true,RegisterDate=DateTime.Now};
                var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email, FristName = Input.Name, LastName = Input.Family, PhoneNumber = Input.PhoneNumber, BirthDate = convert.ConvertShamsiToMiladi(Input.BirthDate), IsActive = true, RegisterDate = DateTime.Now,EmailConfirmed=true ,TwoFactorEnabled=Input.TwoFactorEnabled};
                var result =await  _userManager.CreateAsync(user,Input.Password);
                
                if ( result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");


                    // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // var callbackUrl = Url.Page(
                    //     "/Account/ConfirmEmail",
                    //     pageHandler: null,
                    //     values: new { userId = user.Id, code = code },
                    //     protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    await _userManager.AddToRolesAsync(user, UserRoles);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            GetRoles = _roleManager.GetAllRoles();
            return Page();
        }
    }
}
